import { apiClient } from '../../../core/api';

export const legalApi = {
    getSections: async (page = 1, pageSize = 50) => {
        const res = await apiClient.get(`/Legal/sections?pageNumber=${page}&pageSize=${pageSize}`);
        return res.data;
    },
    getCaseSections: async (caseId: string) => {
        const res = await apiClient.get(`/Legal/case/${caseId}/sections`);
        return res.data;
    },
    addSectionToCase: async (caseId: string, sectionId: string) => {
        const res = await apiClient.post(`/Legal/case/${caseId}/sections`, { legalSectionId: sectionId });
        return res.data;
    },
    getHearings: async (caseId: string) => {
        const res = await apiClient.get(`/Legal/case/${caseId}/hearings`);
        return res.data;
    },
    createHearing: async (data: any) => {
        const res = await apiClient.post('/Legal/hearings', data);
        return res.data;
    },
    getChargesheet: async (caseId: string) => {
        const res = await apiClient.get(`/Legal/case/${caseId}/chargesheet`);
        return res.data;
    },
    getBailApplications: async (caseId: string) => {
        const res = await apiClient.get(`/Legal/case/${caseId}/bail`);
        return res.data;
    },
};
