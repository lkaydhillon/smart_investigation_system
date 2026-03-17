import { create } from 'zustand';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { apiClient } from '../core/api';

export interface SyncJob {
    id: string;
    endpoint: string;
    method: 'POST' | 'PUT' | 'DELETE';
    payload: any;
    timestamp: string;
    retryCount: number;
}

interface SyncState {
    isOnline: boolean;
    queue: SyncJob[];
    isSyncing: boolean;
    setOnlineStatus: (status: boolean) => void;
    addToQueue: (job: Omit<SyncJob, 'id' | 'timestamp' | 'retryCount'>) => Promise<void>;
    processQueue: () => Promise<void>;
    initializeQueue: () => Promise<void>;
}

export const useSyncStore = create<SyncState>((set, get) => ({
    isOnline: true, // Should be wired to NetInfo in a real app
    queue: [],
    isSyncing: false,

    setOnlineStatus: (status) => {
        set({ isOnline: status });
        if (status) {
            get().processQueue();
        }
    },

    addToQueue: async (jobData) => {
        const newJob: SyncJob = {
            ...jobData,
            id: Math.random().toString(36).substring(7),
            timestamp: new Date().toISOString(),
            retryCount: 0
        };

        const newQueue = [...get().queue, newJob];
        set({ queue: newQueue });
        await AsyncStorage.setItem('sync.queue', JSON.stringify(newQueue));
    },

    processQueue: async () => {
        if (!get().isOnline || get().isSyncing || get().queue.length === 0) return;

        set({ isSyncing: true });

        const currentQueue = [...get().queue];
        const failedJobs: SyncJob[] = [];

        for (const job of currentQueue) {
            try {
                await apiClient.request({
                    method: job.method,
                    url: job.endpoint,
                    data: job.payload,
                });
                // Job succeeded, drop from queue
            } catch (error) {
                // Increment retry count
                if (job.retryCount < 3) {
                    failedJobs.push({ ...job, retryCount: job.retryCount + 1 });
                }
                // If > 3 retries, drop the job permanently or log alert
            }
        }

        set({ queue: failedJobs, isSyncing: false });
        await AsyncStorage.setItem('sync.queue', JSON.stringify(failedJobs));
    },

    initializeQueue: async () => {
        const storedQueue = await AsyncStorage.getItem('sync.queue');
        if (storedQueue) {
            set({ queue: JSON.parse(storedQueue) });
        }
    }
}));
