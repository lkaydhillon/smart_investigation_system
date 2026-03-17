import { apiClient } from '../../../core/api';

export const documentsApi = {
    getTemplates: async () => {
        const res = await apiClient.get('/Documents/templates');
        return res.data;
    },
    generateDocument: async (data: any) => {
        const res = await apiClient.post('/Documents/generate', data);
        return res.data;
    },
    getCaseDocuments: async (caseId: string) => {
        const res = await apiClient.get(`/Documents/case/${caseId}`);
        return res.data;
    },
};
