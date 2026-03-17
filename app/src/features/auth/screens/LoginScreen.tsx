// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, TouchableOpacity, TextInput, ActivityIndicator, Alert, StyleSheet, StatusBar, KeyboardAvoidingView, Platform } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useAuthStore } from '../../../store/authStore';
import { authApi } from '../api/authApi';
import { useMutation } from '@tanstack/react-query';

export const LoginScreen = () => {
    const insets = useSafeAreaInsets();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const loginAction = useAuthStore(state => state.login);

    const loginMutation = useMutation({
        mutationFn: authApi.login,
        onSuccess: (data) => {
            loginAction(data.accessToken, {
                id: data.user.id,
                username: data.user.username,
                fullName: data.user.fullName || data.user.username,
                role: data.user.roles[0] ?? 'User',
                email: data.user.email,
                rank: data.user.rank,
                badgeNumber: data.user.badgeNumber
            });
        },
        onError: (error: any) => {
            const message = error.response?.data?.error || error.response?.data?.message || 'Unable to login. Check your credentials or network.';
            Alert.alert('Login Failed', message);
        }
    });

    const handleLogin = () => {
        if (!username || !password) {
            Alert.alert('Required', 'Please enter both badge number and password.');
            return;
        }
        loginMutation.mutate({ username, password });
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#0F172A" />
            <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : undefined} style={s.inner}>
                {/* Logo Area */}
                <View style={s.logoArea}>
                    <View style={s.badge}>
                        <Text style={s.badgeIcon}>🛡️</Text>
                    </View>
                    <Text style={s.appName}>Smart Investigation</Text>
                    <Text style={s.appTagline}>AI-Enabled Case Management System</Text>
                </View>

                {/* Login Card */}
                <View style={s.card}>
                    <Text style={s.cardTitle}>Officer Login</Text>

                    <Text style={s.label}>Badge Number / Username</Text>
                    <TextInput style={s.input} placeholder="e.g. admin" placeholderTextColor="#94A3B8" value={username} onChangeText={setUsername} autoCapitalize="none" autoCorrect={false} />

                    <Text style={s.label}>Password</Text>
                    <TextInput style={s.input} placeholder="Enter password" placeholderTextColor="#94A3B8" secureTextEntry value={password} onChangeText={setPassword} />

                    <TouchableOpacity onPress={handleLogin} disabled={loginMutation.isPending} style={[s.loginBtn, loginMutation.isPending && { opacity: 0.7 }]}>
                        {loginMutation.isPending && <ActivityIndicator color="#fff" style={{ marginRight: 8 }} />}
                        <Text style={s.loginText}>Secure Login</Text>
                    </TouchableOpacity>

                    <Text style={s.hint}>Default: admin / admin123</Text>
                </View>

                <Text style={s.footer}>Powered by AI • v2.0</Text>
            </KeyboardAvoidingView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#0F172A' },
    inner: { flex: 1, justifyContent: 'center', alignItems: 'center', paddingHorizontal: 24 },
    logoArea: { alignItems: 'center', marginBottom: 32 },
    badge: { width: 72, height: 72, borderRadius: 20, backgroundColor: 'rgba(59,130,246,0.15)', justifyContent: 'center', alignItems: 'center', marginBottom: 16 },
    badgeIcon: { fontSize: 36 },
    appName: { fontSize: 26, fontWeight: '800', color: '#fff' },
    appTagline: { fontSize: 13, color: '#64748B', marginTop: 6 },
    card: { width: '100%', backgroundColor: '#1E293B', borderRadius: 20, padding: 24, borderWidth: 1, borderColor: '#334155' },
    cardTitle: { fontSize: 18, fontWeight: '700', color: '#fff', marginBottom: 20, textAlign: 'center' },
    label: { fontSize: 13, fontWeight: '600', color: '#94A3B8', marginBottom: 6, marginTop: 4 },
    input: { backgroundColor: '#0F172A', borderRadius: 12, padding: 14, fontSize: 15, color: '#fff', borderWidth: 1, borderColor: '#334155', marginBottom: 12 },
    loginBtn: { backgroundColor: '#2563EB', borderRadius: 12, padding: 16, alignItems: 'center', flexDirection: 'row', justifyContent: 'center', marginTop: 8 },
    loginText: { color: '#fff', fontWeight: '700', fontSize: 16 },
    hint: { color: '#475569', fontSize: 12, textAlign: 'center', marginTop: 16 },
    footer: { color: '#334155', fontSize: 12, marginTop: 40 },
});
