import { apiClient } from '../../../core/api';

export const personsApi = {
    getAll: async (page = 1, pageSize = 20) => {
        const res = await apiClient.get(`/Persons?pageNumber=${page}&pageSize=${pageSize}`);
        return res.data;
    },
    getById: async (id: string) => {
        const res = await apiClient.get(`/Persons/${id}`);
        return res.data;
    },
    create: async (data: any) => {
        const res = await apiClient.post('/Persons', data);
        return res.data;
    },
    update: async (id: string, data: any) => {
        const res = await apiClient.put(`/Persons/${id}`, data);
        return res.data;
    },
    delete: async (id: string) => {
        const res = await apiClient.delete(`/Persons/${id}`);
        return res.data;
    },
    search: async (query: string) => {
        const res = await apiClient.get(`/Persons/search?query=${encodeURIComponent(query)}`);
        return res.data;
    },
};
