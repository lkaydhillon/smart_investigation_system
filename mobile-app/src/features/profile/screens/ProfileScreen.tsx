// @ts-nocheck
import React from 'react';
import { View, Text, ScrollView, TouchableOpacity, StyleSheet, StatusBar, Alert } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useAuthStore } from '../../../store/authStore';

export const ProfileScreen = () => {
    const insets = useSafeAreaInsets();
    const user = useAuthStore(s => s.user);
    const logout = useAuthStore(s => s.logout);

    const profileFields = [
        { label: 'Name', value: user?.fullName || user?.username || 'Officer', icon: '👤' },
        { label: 'Badge / Username', value: user?.badgeNumber || user?.username || '-', icon: '🪪' },
        { label: 'Role', value: user?.role === 'StationHouseOfficer' ? 'Station House Officer (SHO)' : user?.role === 'InvestigatingOfficer' ? 'Investigating Officer (IO)' : user?.role || 'User', icon: '⭐' },
        { label: 'Rank', value: user?.rank || 'Sub-Inspector', icon: '🎖️' },
        { label: 'Email', value: user?.email || 'N/A', icon: '📧' },
        { label: 'Station', value: 'Parliament Street PS', icon: '🏢' },
        { label: 'District', value: 'New Delhi', icon: '📍' },
    ];

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#1E293B" />
            <View style={s.header}>
                <View style={s.avatarCircle}><Text style={s.avatarText}>{(user?.fullName || user?.username || 'O')[0].toUpperCase()}</Text></View>
                <Text style={s.name}>{user?.fullName || user?.username || 'Officer'}</Text>
                <Text style={s.role}>{user?.rank ? `${user.rank} • ` : ''}{user?.role || 'User'}</Text>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {profileFields.map(f => (
                    <View key={f.label} style={s.fieldCard}>
                        <Text style={{ fontSize: 20, marginRight: 14 }}>{f.icon}</Text>
                        <View>
                            <Text style={s.fieldLabel}>{f.label}</Text>
                            <Text style={s.fieldValue}>{f.value}</Text>
                        </View>
                    </View>
                ))}

                <TouchableOpacity onPress={() => Alert.alert('Coming Soon', 'Settings module under development.')} style={s.settingsBtn}>
                    <Text style={s.settingsBtnText}>⚙️ App Settings</Text>
                </TouchableOpacity>

                <TouchableOpacity onPress={logout} style={s.logoutBtn}>
                    <Text style={s.logoutBtnText}>Logout</Text>
                </TouchableOpacity>
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#1E293B', paddingHorizontal: 20, paddingTop: 24, paddingBottom: 24, alignItems: 'center' },
    avatarCircle: { width: 72, height: 72, borderRadius: 36, backgroundColor: '#3B82F6', justifyContent: 'center', alignItems: 'center', marginBottom: 12 },
    avatarText: { fontSize: 32, fontWeight: '800', color: '#fff' },
    name: { fontSize: 20, fontWeight: '800', color: '#fff' },
    role: { fontSize: 13, color: '#93C5FD', marginTop: 4 },
    body: { flex: 1, padding: 16 },
    fieldCard: { backgroundColor: '#fff', borderRadius: 12, padding: 16, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0', flexDirection: 'row', alignItems: 'center' },
    fieldLabel: { fontSize: 11, color: '#94A3B8', fontWeight: '600' },
    fieldValue: { fontSize: 15, fontWeight: '700', color: '#0F172A', marginTop: 2 },
    settingsBtn: { backgroundColor: '#F1F5F9', padding: 16, borderRadius: 12, alignItems: 'center', marginTop: 16, borderWidth: 1, borderColor: '#E2E8F0' },
    settingsBtnText: { fontSize: 15, fontWeight: '600', color: '#475569' },
    logoutBtn: { backgroundColor: '#DC2626', padding: 16, borderRadius: 12, alignItems: 'center', marginTop: 12 },
    logoutBtnText: { color: '#fff', fontWeight: '700', fontSize: 16 },
});
