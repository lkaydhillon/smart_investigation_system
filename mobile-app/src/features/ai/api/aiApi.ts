import { apiClient, API_BASE_URL } from '../../../core/api';
import axios from 'axios';

// AI Controller is on /api/ (not /api/v1/) based on AIController route
const aiBaseUrl = API_BASE_URL.replace('/api/v1', '/api');

export const aiApi = {
    recommendSections: async (caseDescription: string) => {
        const res = await axios.post(`${aiBaseUrl}/AI/recommend-sections`, { caseDescription }, {
            headers: { 'Content-Type': 'application/json', Authorization: apiClient.defaults.headers.common['Authorization'] || '' },
            timeout: 30000,
        });
        return res.data;
    },
    draftDocument: async (documentType: string, contextData: any) => {
        const res = await axios.post(`${aiBaseUrl}/AI/draft-document`, { documentType, contextData }, {
            headers: { 'Content-Type': 'application/json', Authorization: apiClient.defaults.headers.common['Authorization'] || '' },
            timeout: 30000,
        });
        return res.data;
    },
    generateInterrogationQuestions: async (caseFacts: string, personRole: string) => {
        // Simulated AI endpoint - in production this would call an LLM
        return {
            questions: [
                `Where were you on the date of the incident described: "${caseFacts.substring(0, 50)}..."?`,
                `Do you know anyone involved in this matter?`,
                `Can you provide an alibi for your whereabouts during the incident?`,
                `Have you had any prior interactions with the complainant?`,
                `Is there any evidence (messages, calls, witnesses) that can corroborate your statement?`,
                `Do you wish to make a voluntary statement under Section 161 CrPC?`,
            ]
        };
    },
};
