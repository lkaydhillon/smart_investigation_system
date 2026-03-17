import axios from 'axios';
import { useAuthStore } from '../store/authStore';
import { syncService } from './database/OfflineSyncService';
import NetInfo from '@react-native-community/netinfo';

// Physical devices must hit the machine's LAN IP, not localhost.
// const DEV_URL = 'http://10.250.93.64:5000/api/v1';

// --- Production URLs ---
// const PROD_URL = 'https://smart-investigation-backend.onrender.com/api/v1'; 
const PROD_URL = 'https://smart-investigation-system.onrender.com/api/v1';

export const API_BASE_URL = __DEV__ ? PROD_URL : PROD_URL; // Force PROD_URL for Web testing/Vercel

export const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

apiClient.interceptors.request.use(async (config) => {
    const token = useAuthStore.getState().token;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Configure Axios to cache failed MUTATION requests offline
apiClient.interceptors.response.use(
    (response) => response,
    async (error) => {
        const config = error.config;

        // If it's a mutation (POST, PUT, PATCH, DELETE) and network failed
        // NEVER queue auth endpoints — they need real responses
        const isAuthEndpoint = config?.url?.includes('/auth/') || config?.url?.includes('/Auth/');
        if (config && !isAuthEndpoint && (config.method === 'post' || config.method === 'put' || config.method === 'patch' || config.method === 'delete')) {
            const networkState = await NetInfo.fetch();

            if (!networkState.isConnected || error.message === 'Network request failed' || error.message?.includes('Network Error') || error.code === 'ERR_NETWORK') {
                console.warn('[Network Interceptor] Offline mutation detected. Queuing to SQLite DB for background sync.');

                // Construct the full URL for the queuing system
                const fullUrl = config.baseURL ? `${config.baseURL}${config.url}` : config.url;

                await syncService.queueOperation(
                    fullUrl,
                    config.method.toUpperCase(),
                    config.data ? JSON.parse(config.data) : undefined, // Safe parse back if axios stringified
                    config.headers
                );

                // We mock a successful "accepted" response so the UI optimistically updates
                return Promise.resolve({
                    data: { _optimistic: true, message: 'Saved offline. Will sync when reconnected.' },
                    status: 202,
                    statusText: 'Accepted (Offline Mode)',
                    headers: {},
                    config,
                });
            }
        }

        if (error.response?.status === 401) {
            useAuthStore.getState().logout();
        }
        return Promise.reject(error);
    }
);
