import * as SQLite from 'expo-sqlite';
import NetInfo from '@react-native-community/netinfo';
import { AppState } from 'react-native';

export interface SyncOperation {
    id?: number;
    url: string;
    method: string;
    body: string;
    headers: string;
    createdAt: number;
    status: 'pending' | 'failed' | 'processing';
}

class OfflineSyncService {
    private db: SQLite.SQLiteDatabase | null = null;
    private isSyncing = false;

    async init() {
        this.db = await SQLite.openDatabaseAsync('smart_investigation.db');

        await this.db.execAsync(`
            CREATE TABLE IF NOT EXISTS sync_queue (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                url TEXT NOT NULL,
                method TEXT NOT NULL,
                body TEXT,
                headers TEXT,
                createdAt INTEGER,
                status TEXT DEFAULT 'pending'
            );
        `);

        // Watch for network connectivity changes
        NetInfo.addEventListener(state => {
            if (state.isConnected && state.isInternetReachable) {
                this.syncPendingOperations();
            }
        });

        // Watch for app coming back to foreground
        AppState.addEventListener('change', nextAppState => {
            if (nextAppState === 'active') {
                this.syncPendingOperations();
            }
        });
    }

    async queueOperation(url: string, method: string, data?: any, headers?: any) {
        if (!this.db) await this.init();

        try {
            await this.db!.runAsync(
                `INSERT INTO sync_queue (url, method, body, headers, createdAt, status) VALUES (?, ?, ?, ?, ?, 'pending')`,
                [url, method, data ? JSON.stringify(data) : null, headers ? JSON.stringify(headers) : null, Date.now()]
            );
            console.log('[OfflineSync] Operation queued for background sync:', url);
        } catch (e) {
            console.error('[OfflineSync] Failed to queue operation', e);
        }
    }

    async syncPendingOperations() {
        if (!this.db || this.isSyncing) return;

        const networkState = await NetInfo.fetch();
        if (!networkState.isConnected || !networkState.isInternetReachable) return;

        this.isSyncing = true;

        try {
            // Lock records to prevent double-processing
            const pendingOps = await this.db.getAllAsync<SyncOperation>(
                `SELECT * FROM sync_queue WHERE status = 'pending' ORDER BY createdAt ASC`
            );

            if (pendingOps.length === 0) {
                this.isSyncing = false;
                return;
            }

            console.log(`[OfflineSync] Found ${pendingOps.length} pending operations. Starting sync...`);

            for (const op of pendingOps) {
                await this.db.runAsync(`UPDATE sync_queue SET status = 'processing' WHERE id = ?`, [op.id!]);

                try {
                    const response = await fetch(op.url, {
                        method: op.method,
                        headers: op.headers ? JSON.parse(op.headers) : { 'Content-Type': 'application/json' },
                        body: op.body ? op.body : undefined,
                    });

                    if (response.ok) {
                        await this.db.runAsync(`DELETE FROM sync_queue WHERE id = ?`, [op.id!]);
                        console.log(`[OfflineSync] Successfully synced operation ID: ${op.id}`);
                    } else {
                        throw new Error(`HTTP Error ${response.status}`);
                    }
                } catch (e) {
                    console.error(`[OfflineSync] Operation ID ${op.id} failed to sync. Will retry later.`, e);
                    await this.db.runAsync(`UPDATE sync_queue SET status = 'pending' WHERE id = ?`, [op.id!]);
                }
            }
        } finally {
            this.isSyncing = false;
        }
    }
}

export const syncService = new OfflineSyncService();
