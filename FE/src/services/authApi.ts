import api from './api';
import { useAuthStore } from '../store/authStore';

export const authApi = {
    login: async (request: any) => {
        try {
            const response = await api.post('/Auth/login', request);
            if (response.data) {
                const { accessToken, user } = response.data;
                // In the backend DTO, user contains roles. Let's map it to our UI-friendly UserRole.
                const mappedRole = user.roles?.includes('SuperAdmin') ? 'SuperAdmin' :
                    (user.roles?.includes('Admin') ? 'Admin' : 'Investigator');

                useAuthStore.getState().login({
                    id: user.id,
                    username: user.username,
                    fullName: user.fullName || user.username,
                    role: mappedRole,
                    badgeNumber: user.badgeNumber,
                    rank: user.rank,
                    policeStationId: user.policeStationId
                }, accessToken);
            }
            return response.data;
        } catch (error: any) {
            throw error.response?.data?.error || 'Login failed. Please check credentials.';
        }
    },

    logout: async () => {
        try {
            await api.post('/Auth/logout');
        } finally {
            useAuthStore.getState().logout();
        }
    }
};
