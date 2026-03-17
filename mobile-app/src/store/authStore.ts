import { create } from 'zustand';
import AsyncStorage from '@react-native-async-storage/async-storage';

interface AuthState {
    token: string | null;
    user: { id: string; username: string; role: string; fullName?: string; email?: string; rank?: string; badgeNumber?: string } | null;
    isAuthenticated: boolean;
    login: (token: string, user: { id: string; username: string; role: string; fullName?: string; email?: string; rank?: string; badgeNumber?: string }) => Promise<void>;
    logout: () => Promise<void>;
    initializeAuth: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
    token: null,
    user: null,
    isAuthenticated: false,

    login: async (token, user) => {
        await AsyncStorage.setItem('auth.token', token);
        await AsyncStorage.setItem('auth.user', JSON.stringify(user));
        set({ token, user, isAuthenticated: true });
    },

    logout: async () => {
        await AsyncStorage.removeItem('auth.token');
        await AsyncStorage.removeItem('auth.user');
        set({ token: null, user: null, isAuthenticated: false });
    },

    initializeAuth: async () => {
        const token = await AsyncStorage.getItem('auth.token');
        const userStr = await AsyncStorage.getItem('auth.user');
        if (token && userStr) {
            set({ token, user: JSON.parse(userStr), isAuthenticated: true });
        }
    }
}));
