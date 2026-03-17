import { apiClient } from '../../../core/api';

export interface LoginRequest {
    username: string;
    password: string;
    deviceId?: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    user: {
        id: string;
        username: string;
        roles: string[];
    };
}

export const authApi = {
    login: async (credentials: LoginRequest): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/auth/login', credentials);
        return response.data;
    },
};
