import { apiClient } from '../../../core/api';

export const investigationApi = {
    getSteps: async (caseId: string) => {
        const res = await apiClient.get(`/Investigation/case/${caseId}/steps`);
        return res.data;
    },
    updateStep: async (stepId: string, data: any) => {
        const res = await apiClient.put(`/Investigation/steps/${stepId}`, data);
        return res.data;
    },
    getTeam: async (caseId: string) => {
        const res = await apiClient.get(`/Investigation/case/${caseId}/team`);
        return res.data;
    },
    getDiary: async (caseId: string) => {
        const res = await apiClient.get(`/Investigation/case/${caseId}/diary`);
        return res.data;
    },
    addDiaryEntry: async (caseId: string, data: any) => {
        const res = await apiClient.post(`/Investigation/case/${caseId}/diary`, data);
        return res.data;
    },
    getInterrogations: async (caseId: string) => {
        const res = await apiClient.get(`/Investigation/case/${caseId}/interrogations`);
        return res.data;
    },
    createInterrogation: async (data: any) => {
        const res = await apiClient.post('/Investigation/interrogation', data);
        return res.data;
    },
};
