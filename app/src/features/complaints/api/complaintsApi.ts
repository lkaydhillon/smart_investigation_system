import { apiClient } from '../../../core/api';

export const complaintsApi = {
    getAll: async (page = 1, pageSize = 20) => {
        const res = await apiClient.get(`/Complaints?pageNumber=${page}&pageSize=${pageSize}`);
        return res.data;
    },
    getById: async (id: string) => {
        const res = await apiClient.get(`/Complaints/${id}`);
        return res.data;
    },
    create: async (data: any) => {
        const res = await apiClient.post('/Complaints', data);
        return res.data;
    },
    update: async (id: string, data: any) => {
        const res = await apiClient.put(`/Complaints/${id}`, data);
        return res.data;
    },
    delete: async (id: string) => {
        const res = await apiClient.delete(`/Complaints/${id}`);
        return res.data;
    },
};
