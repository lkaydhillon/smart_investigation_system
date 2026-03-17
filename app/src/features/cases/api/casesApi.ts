import { apiClient } from '../../../core/api';

export interface CaseSummary {
    id: string;
    caseNumber: string;
    firId?: string;
    status: string;
    priority: string;
    crimeTypeId: string;
    stationId: string;
    createdDate: string;
}

export const casesApi = {
    getDashboardSummary: async () => {
        // Assuming backend has an endpoint for dashboard statistics and cases 
        // This calls the generic active tasks or case list endpoint. 
        // Fallback to basic case listing if dashboard specifically doesn't exist yet.
        // In Phase 1, we made /api/Cases which returns PaginatedResponse
        const response = await apiClient.get('/cases?pageSize=5');
        return response.data;
    },

    getCaseDetails: async (id: string) => {
        const response = await apiClient.get(`/cases/${id}`);
        return response.data;
    }
};
