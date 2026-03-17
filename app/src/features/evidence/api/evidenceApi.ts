import { apiClient } from '../../../core/api';

export interface UploadEvidenceRequest {
    caseId: string;
    type: number;
    description: string;
    collectedBy?: string;
    collectionLocation?: string;
    gpsLatitude?: number;
    gpsLongitude?: number;
    categoryId?: string;
}

export const evidenceApi = {
    uploadEvidence: async (data: UploadEvidenceRequest) => {
        const response = await apiClient.post('/Evidence', data);
        return response.data;
    }
};
