// @ts-nocheck
import React, { useEffect } from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { AuthNavigator } from './AuthNavigator';
import { MainTabNavigator } from './MainTabNavigator';
import { useAuthStore } from '../store/authStore';
import { View, ActivityIndicator } from 'react-native';

export type RootStackParamList = {
    Auth: undefined;
    Main: undefined;
};

const Stack = createNativeStackNavigator<RootStackParamList>();

import { syncService } from '../core/database/OfflineSyncService';

export const RootNavigator = () => {
    const { isAuthenticated, initializeAuth } = useAuthStore();
    const [isReady, setIsReady] = React.useState(false);

    useEffect(() => {
        const init = async () => {
            await initializeAuth();
            await syncService.init(); // Boot the SQLite DB and Network connection listeners
            setIsReady(true);
        };
        init();
    }, [initializeAuth]);

    if (!isReady) {
        return (
            <View className="flex-1 justify-center items-center bg-white">
                <ActivityIndicator size="large" color="#2563EB" />
            </View>
        );
    }

    return (
        <Stack.Navigator screenOptions={{ headerShown: false }}>
            {isAuthenticated ? (
                <Stack.Screen name="Main" component={MainTabNavigator} />
            ) : (
                <Stack.Screen name="Auth" component={AuthNavigator} />
            )}
        </Stack.Navigator>
    );
};
