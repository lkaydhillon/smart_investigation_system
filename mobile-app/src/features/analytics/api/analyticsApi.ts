import { apiClient } from '../../../core/api';

export const analyticsApi = {
    getDashboardStats: async () => {
        const res = await apiClient.get('/Analytics/dashboard');
        return res.data;
    },
    getHotspots: async () => {
        const res = await apiClient.get('/Analytics/hotspots');
        return res.data;
    },
};
