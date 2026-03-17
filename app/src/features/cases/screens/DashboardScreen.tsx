// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, ActivityIndicator, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery } from '@tanstack/react-query';
import { casesApi } from '../api/casesApi';
import { apiClient } from '../../../core/api';
import { useNavigation } from '@react-navigation/native';
import { useAuthStore } from '../../../store/authStore';

export const DashboardScreen = () => {
    const insets = useSafeAreaInsets();
    const navigation = useNavigation();
    const { user, logout } = useAuthStore();
    const rankDisplay = user?.rank || (user?.role === 'StationHouseOfficer' ? 'SHO' : user?.role === 'InvestigatingOfficer' ? 'IO' : 'Officer');

    const [refreshing, setRefreshing] = useState(false);

    const casesQuery = useQuery({ queryKey: ['cases', 'dashboard'], queryFn: casesApi.getDashboardSummary });
    const complaintsQuery = useQuery({ queryKey: ['complaints-dash'], queryFn: async () => { const res = await apiClient.get('/Complaints?pageSize=10'); return res.data; } });

    const cases = casesQuery.data?.items || [];
    const complaints = complaintsQuery.data?.items || [];
    const urgent = complaints.filter((c: any) => c.isUrgent).length;

    const quickActions = [
        { icon: '📝', label: 'Complaint', screen: 'Complaints', parent: 'More' },
        { icon: '📸', label: 'Evidence', screen: 'Evidence', parent: null },
        { icon: '🎤', label: 'Voice', screen: 'VoiceRecording', parent: 'More' },
        { icon: '🤖', label: 'AI', screen: 'AIAssistant', parent: 'More' },
        { icon: '🔎', label: 'Search', screen: 'Search', parent: 'More' },
        { icon: '📈', label: 'Reports', screen: 'Supervisory', parent: 'More' },
        { icon: '⏰', label: 'Alerts', screen: 'Compliance', parent: 'More' },
        { icon: '📋', label: 'Challan', screen: 'ChallanScrutiny', parent: 'More' },
    ];

    const getPriorityColor = (p: string) => {
        if (p === 'Critical') return '#DC2626';
        if (p === 'High') return '#EA580C';
        if (p === 'Medium') return '#CA8A04';
        return '#16A34A';
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#1E3A5F" />
            {/* Header */}
            <View style={s.header}>
                <View style={s.headerTop}>
                    <View>
                        <Text style={s.greeting}>Good Morning, {rankDisplay}</Text>
                        <Text style={s.officerName}>{user?.fullName || user?.username || 'Officer'}</Text>
                    </View>
                    <View style={s.avatar}><Text style={s.avatarText}>{(user?.fullName || user?.username || 'O')[0].toUpperCase()}</Text></View>
                </View>
                <View style={s.statsRow}>
                    <View style={s.statBox}>
                        <Text style={s.statLabel}>Active Cases</Text>
                        <Text style={s.statValue}>{cases.length}</Text>
                    </View>
                    <View style={s.statBox}>
                        <Text style={s.statLabel}>Complaints</Text>
                        <Text style={s.statValue}>{complaints.length}</Text>
                    </View>
                    <View style={s.statBox}>
                        <Text style={s.statLabel}>Urgent</Text>
                        <Text style={[s.statValue, { color: '#FF6B6B' }]}>{urgent}</Text>
                    </View>
                </View>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* Quick Actions */}
                <Text style={s.sectionTitle}>Quick Actions</Text>
                <ScrollView horizontal showsHorizontalScrollIndicator={false} style={{ marginBottom: 20 }}>
                    {quickActions.map(a => (
                        <TouchableOpacity key={a.label}
                            onPress={() => a.parent ? navigation.navigate(a.parent, { screen: a.screen }) : navigation.navigate(a.screen)}
                            style={s.actionCard}>
                            <Text style={{ fontSize: 24, marginBottom: 4 }}>{a.icon}</Text>
                            <Text style={s.actionLabel}>{a.label}</Text>
                        </TouchableOpacity>
                    ))}
                </ScrollView>

                {/* Recent Cases */}
                <View style={s.sectionHeader}>
                    <Text style={s.sectionTitle}>Recent Cases</Text>
                    <TouchableOpacity onPress={() => navigation.navigate('Cases')}>
                        <Text style={s.seeAll}>View All →</Text>
                    </TouchableOpacity>
                </View>

                {casesQuery.isLoading && <ActivityIndicator size="large" color="#1D4ED8" />}

                {cases.slice(0, 5).map((c: any) => (
                    <TouchableOpacity key={c.id}
                        onPress={() => navigation.navigate('Cases', { screen: 'CaseDetail', params: { caseId: c.id } })}
                        style={s.caseCard}>
                        <View style={s.caseCardTop}>
                            <Text style={s.caseNumber}>{c.caseNumber}</Text>
                            <View style={[s.priorityBadge, { backgroundColor: getPriorityColor(c.priority) + '20' }]}>
                                <Text style={[s.priorityText, { color: getPriorityColor(c.priority) }]}>{c.priority}</Text>
                            </View>
                        </View>
                        <Text style={s.caseTitle} numberOfLines={1}>{c.title || 'Untitled'}</Text>
                        <View style={s.caseCardBottom}>
                            <View style={s.statusRow}>
                                <View style={[s.statusDot, { backgroundColor: c.status === 'Registered' ? '#3B82F6' : '#F59E0B' }]} />
                                <Text style={s.statusText}>{c.status}</Text>
                            </View>
                            <Text style={s.dateText}>{new Date(c.createdDate).toLocaleDateString('en-IN')}</Text>
                        </View>
                    </TouchableOpacity>
                ))}

                {/* Recent Complaints */}
                {complaints.length > 0 && (
                    <>
                        <View style={[s.sectionHeader, { marginTop: 8 }]}>
                            <Text style={s.sectionTitle}>Recent Complaints</Text>
                            <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'Complaints' })}>
                                <Text style={s.seeAll}>View All →</Text>
                            </TouchableOpacity>
                        </View>
                        {complaints.slice(0, 3).map((c: any) => (
                            <View key={c.id} style={s.complaintCard}>
                                <View style={s.caseCardTop}>
                                    <Text style={s.caseNumber}>{c.complaintNumber}</Text>
                                    {c.isUrgent && <View style={[s.priorityBadge, { backgroundColor: '#FEE2E2' }]}><Text style={{ fontSize: 11, fontWeight: '700', color: '#DC2626' }}>URGENT</Text></View>}
                                </View>
                                <Text style={s.caseTitle} numberOfLines={1}>{c.description}</Text>
                                <Text style={s.dateText}>{c.complainantName} • {new Date(c.createdDate).toLocaleDateString('en-IN')}</Text>
                            </View>
                        ))}
                    </>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#1E3A5F', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 20 },
    headerTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
    greeting: { color: '#94A3B8', fontSize: 13, fontWeight: '600' },
    officerName: { color: '#fff', fontSize: 22, fontWeight: '800', marginTop: 2 },
    avatar: { width: 44, height: 44, borderRadius: 22, backgroundColor: '#3B82F6', justifyContent: 'center', alignItems: 'center', borderWidth: 2, borderColor: '#fff' },
    avatarText: { color: '#fff', fontSize: 18, fontWeight: '800' },
    logoutBtn: { backgroundColor: 'rgba(255,255,255,0.15)', paddingHorizontal: 12, paddingVertical: 6, borderRadius: 8 },
    logoutText: { color: '#93C5FD', fontSize: 12, fontWeight: '600' },
    statsRow: { flexDirection: 'row', marginTop: 16, gap: 10 },
    statBox: { flex: 1, backgroundColor: 'rgba(255,255,255,0.1)', padding: 12, borderRadius: 12 },
    statLabel: { fontSize: 11, color: '#93C5FD', fontWeight: '600' },
    statValue: { fontSize: 28, fontWeight: '800', color: '#fff', marginTop: 2 },
    body: { flex: 1, paddingHorizontal: 16, paddingTop: 20 },
    sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 },
    sectionTitle: { fontSize: 16, fontWeight: '700', color: '#0F172A', marginBottom: 12 },
    seeAll: { fontSize: 13, fontWeight: '600', color: '#1D4ED8' },
    actionCard: { backgroundColor: '#fff', width: 76, height: 76, borderRadius: 14, alignItems: 'center', justifyContent: 'center', marginRight: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1, shadowColor: '#000', shadowOpacity: 0.04, shadowRadius: 4, shadowOffset: { width: 0, height: 1 } },
    actionLabel: { fontSize: 11, fontWeight: '600', color: '#475569' },
    caseCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1, shadowColor: '#000', shadowOpacity: 0.03, shadowRadius: 4, shadowOffset: { width: 0, height: 1 } },
    caseCardTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 6 },
    caseNumber: { fontSize: 14, fontWeight: '700', color: '#0F172A' },
    caseTitle: { fontSize: 14, color: '#475569', marginBottom: 8 },
    caseCardBottom: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
    statusRow: { flexDirection: 'row', alignItems: 'center' },
    statusDot: { width: 8, height: 8, borderRadius: 4, marginRight: 6 },
    statusText: { fontSize: 12, color: '#64748B', fontWeight: '500' },
    dateText: { fontSize: 12, color: '#94A3B8' },
    priorityBadge: { paddingHorizontal: 10, paddingVertical: 3, borderRadius: 8 },
    priorityText: { fontSize: 11, fontWeight: '700' },
    complaintCard: { backgroundColor: '#fff', borderRadius: 12, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0' },
});
