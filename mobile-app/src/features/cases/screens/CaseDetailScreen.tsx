// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, ActivityIndicator, Alert, TextInput, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useRoute, useNavigation } from '@react-navigation/native';
import { casesApi } from '../api/casesApi';
import { apiClient } from '../../../core/api';

export const CaseDetailScreen = () => {
    const insets = useSafeAreaInsets();
    const route = useRoute();
    const navigation = useNavigation();
    const queryClient = useQueryClient();
    const { caseId } = route.params as { caseId: string };
    const [activeTab, setActiveTab] = useState('overview');
    const [diaryEntry, setDiaryEntry] = useState('');

    const { data: caseItem, isLoading, isError } = useQuery({
        queryKey: ['case', caseId],
        queryFn: () => casesApi.getCaseDetails(caseId),
    });

    const diaryQuery = useQuery({
        queryKey: ['diary', caseId],
        queryFn: async () => { const res = await apiClient.get(`/Investigation/diary/${caseId}?pageSize=50`); return res.data; },
        enabled: activeTab === 'diary',
    });

    const addDiaryMutation = useMutation({
        mutationFn: async () => {
            const res = await apiClient.post('/Investigation/diary', { caseId, content: diaryEntry, attachmentUrls: null });
            return res.data;
        },
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['diary', caseId] }); setDiaryEntry(''); Alert.alert('✅ Success', 'Diary entry added successfully'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to add diary entry'),
    });

    if (isLoading) return <View style={[s.loadingContainer, { paddingTop: insets.top }]}><ActivityIndicator size="large" color="#1D4ED8" /></View>;
    if (isError || !caseItem) return (
        <View style={[s.loadingContainer, { paddingTop: insets.top }]}>
            <TouchableOpacity onPress={() => navigation.goBack()} style={{ marginBottom: 20 }}><Text style={s.backText}>← Back</Text></TouchableOpacity>
            <Text style={{ color: '#DC2626', fontSize: 16 }}>Failed to load case.</Text>
        </View>
    );

    const tabs = [
        { key: 'overview', label: 'Overview' },
        { key: 'diary', label: 'Diary' },
        { key: 'persons', label: 'Persons' },
        { key: 'evidence', label: 'Evidence' },
        { key: 'legal', label: 'Legal' },
    ];
    const diaryEntries = diaryQuery.data?.items || [];

    const getPriorityColor = (p: string) => {
        if (p === 'Critical') return '#DC2626';
        if (p === 'High') return '#EA580C';
        if (p === 'Medium') return '#CA8A04';
        return '#16A34A';
    };

    const pc = getPriorityColor(caseItem.priority);

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="dark-content" backgroundColor="#fff" />
            {/* Header */}
            <View style={s.header}>
                <View style={s.headerRow}>
                    <TouchableOpacity onPress={() => navigation.goBack()} style={s.backBtn}>
                        <Text style={{ fontSize: 18, color: '#1D4ED8' }}>←</Text>
                    </TouchableOpacity>
                    <View style={{ flex: 1 }}>
                        <Text style={s.caseNumber}>{caseItem.caseNumber}</Text>
                        <Text style={s.caseTitle} numberOfLines={1}>{caseItem.title || 'Case Dossier'}</Text>
                    </View>
                    <View style={[s.priorityBadge, { backgroundColor: pc + '20' }]}>
                        <Text style={{ fontSize: 11, fontWeight: '700', color: pc }}>{caseItem.priority}</Text>
                    </View>
                </View>

                {/* Status bar */}
                <View style={s.statusBar}>
                    <View style={s.statusRow}>
                        <View style={[s.dot, { backgroundColor: caseItem.status === 'Registered' ? '#3B82F6' : '#F59E0B' }]} />
                        <Text style={s.statusText}>{caseItem.status}</Text>
                    </View>
                    <Text style={s.dateText}>{new Date(caseItem.createdDate).toLocaleDateString('en-IN')}</Text>
                </View>

                {/* Tabs */}
                <ScrollView horizontal showsHorizontalScrollIndicator={false} style={{ marginTop: 12 }}>
                    {tabs.map(tab => (
                        <TouchableOpacity key={tab.key} onPress={() => setActiveTab(tab.key)}
                            style={[s.tab, activeTab === tab.key && s.tabActive]}>
                            <Text style={[s.tabText, activeTab === tab.key && s.tabTextActive]}>{tab.label}</Text>
                        </TouchableOpacity>
                    ))}
                </ScrollView>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* OVERVIEW */}
                {activeTab === 'overview' && (
                    <>
                        <View style={s.card}>
                            <Text style={s.cardLabel}>Incident Summary</Text>
                            <Text style={s.cardContent}>{caseItem.description || 'No description available.'}</Text>
                        </View>

                        <View style={s.infoGrid}>
                            {[
                                { label: 'Police Station', value: caseItem.stationName || 'Parliament Street' },
                                { label: 'District', value: caseItem.districtName || 'New Delhi' },
                                { label: 'IO Assigned', value: caseItem.assignedOfficerName || 'Unassigned' },
                                { label: 'FIR Number', value: caseItem.firNumber || 'Not Filed' },
                            ].map((info, i) => (
                                <View key={i} style={s.infoItem}>
                                    <Text style={s.infoLabel}>{info.label}</Text>
                                    <Text style={s.infoValue}>{info.value}</Text>
                                </View>
                            ))}
                        </View>

                        <Text style={s.sectionTitle}>Quick Actions</Text>
                        <View style={s.actionsGrid}>
                            <TouchableOpacity onPress={() => navigation.navigate('Evidence')} style={s.actionBtn}>
                                <Text style={s.actionBtnText}>📸 Evidence</Text>
                            </TouchableOpacity>
                            <TouchableOpacity onPress={() => setActiveTab('diary')} style={[s.actionBtn, s.actionBtnOutline]}>
                                <Text style={s.actionOutlineText}>📖 Diary</Text>
                            </TouchableOpacity>
                            <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'AIAssistant' })} style={[s.actionBtn, s.actionBtnOutline]}>
                                <Text style={s.actionOutlineText}>🤖 AI Sections</Text>
                            </TouchableOpacity>
                            <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'ChallanScrutiny' })} style={[s.actionBtn, s.actionBtnOutline]}>
                                <Text style={s.actionOutlineText}>📋 Challan</Text>
                            </TouchableOpacity>
                        </View>
                    </>
                )}

                {/* DIARY */}
                {activeTab === 'diary' && (
                    <>
                        <View style={s.card}>
                            <TextInput style={s.diaryInput} placeholder="Write a new case diary entry..." multiline numberOfLines={4} textAlignVertical="top" value={diaryEntry} onChangeText={setDiaryEntry} placeholderTextColor="#94A3B8" />
                            <TouchableOpacity onPress={() => addDiaryMutation.mutate()} disabled={!diaryEntry.trim() || addDiaryMutation.isPending}
                                style={[s.submitBtn, (!diaryEntry.trim()) && { backgroundColor: '#CBD5E1' }]}>
                                {addDiaryMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.submitText}>Add Entry</Text>}
                            </TouchableOpacity>
                        </View>
                        {diaryQuery.isLoading && <ActivityIndicator size="large" color="#1D4ED8" />}
                        {diaryEntries.map((entry: any, i: number) => (
                            <View key={entry.id || i} style={s.diaryCard}>
                                <Text style={s.cardContent}>{entry.content}</Text>
                                <Text style={s.dateMeta}>{entry.officerName || 'IO'} • {new Date(entry.createdDate || Date.now()).toLocaleDateString('en-IN')}</Text>
                            </View>
                        ))}
                        {!diaryQuery.isLoading && diaryEntries.length === 0 && <Text style={s.emptyText}>No diary entries yet. Add the first one above.</Text>}
                    </>
                )}

                {/* PERSONS */}
                {activeTab === 'persons' && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 48 }}>👥</Text>
                        <Text style={s.emptyTitle}>Linked Persons</Text>
                        <Text style={s.emptyDesc}>Accused, victims and witnesses linked to this case.</Text>
                        <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'Persons' })} style={s.actionBtn}><Text style={s.actionBtnText}>Go to Persons Registry</Text></TouchableOpacity>
                    </View>
                )}

                {/* EVIDENCE */}
                {activeTab === 'evidence' && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 48 }}>📸</Text>
                        <Text style={s.emptyTitle}>Case Evidence</Text>
                        <Text style={s.emptyDesc}>Photos, videos, documents, and forensic reports.</Text>
                        <TouchableOpacity onPress={() => navigation.navigate('Evidence')} style={s.actionBtn}><Text style={s.actionBtnText}>Capture Evidence</Text></TouchableOpacity>
                    </View>
                )}

                {/* LEGAL */}
                {activeTab === 'legal' && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 48 }}>⚖️</Text>
                        <Text style={s.emptyTitle}>Applied Legal Sections</Text>
                        <Text style={s.emptyDesc}>BNS/IPC sections, court hearings, and bail for this case.</Text>
                        <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'Legal' })} style={s.actionBtn}><Text style={s.actionBtnText}>Legal Sections</Text></TouchableOpacity>
                        <TouchableOpacity onPress={() => navigation.navigate('More', { screen: 'AIAssistant' })} style={[s.actionBtn, s.actionBtnOutline, { marginTop: 10 }]}><Text style={s.actionOutlineText}>🤖 AI Recommend</Text></TouchableOpacity>
                    </View>
                )}

                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    loadingContainer: { flex: 1, backgroundColor: '#fff', justifyContent: 'center', alignItems: 'center' },
    header: { backgroundColor: '#fff', paddingHorizontal: 16, paddingTop: 12, paddingBottom: 12, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    headerRow: { flexDirection: 'row', alignItems: 'center' },
    backBtn: { width: 36, height: 36, borderRadius: 10, backgroundColor: '#EFF6FF', justifyContent: 'center', alignItems: 'center', marginRight: 12 },
    backText: { color: '#1D4ED8', fontWeight: '700', fontSize: 15 },
    caseNumber: { fontSize: 16, fontWeight: '800', color: '#0F172A' },
    caseTitle: { fontSize: 13, color: '#64748B' },
    priorityBadge: { paddingHorizontal: 12, paddingVertical: 4, borderRadius: 8 },
    statusBar: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginTop: 10 },
    statusRow: { flexDirection: 'row', alignItems: 'center' },
    dot: { width: 8, height: 8, borderRadius: 4, marginRight: 6 },
    statusText: { fontSize: 13, fontWeight: '600', color: '#475569' },
    dateText: { fontSize: 12, color: '#94A3B8' },
    tab: { paddingHorizontal: 16, paddingVertical: 8, borderRadius: 8, backgroundColor: '#F1F5F9', marginRight: 8 },
    tabActive: { backgroundColor: '#1D4ED8' },
    tabText: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    tabTextActive: { color: '#fff' },
    body: { flex: 1, padding: 16 },
    card: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    cardLabel: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 8 },
    cardContent: { fontSize: 14, color: '#475569', lineHeight: 22 },
    infoGrid: { flexDirection: 'row', flexWrap: 'wrap', marginBottom: 16 },
    infoItem: { width: '48%', backgroundColor: '#fff', padding: 12, borderRadius: 12, borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 8, marginRight: '4%' },
    infoLabel: { fontSize: 11, color: '#94A3B8', fontWeight: '600' },
    infoValue: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginTop: 2 },
    sectionTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 12 },
    actionsGrid: { flexDirection: 'row', flexWrap: 'wrap' },
    actionBtn: { backgroundColor: '#1D4ED8', paddingVertical: 12, paddingHorizontal: 16, borderRadius: 10, marginRight: 8, marginBottom: 8 },
    actionBtnText: { color: '#fff', fontSize: 13, fontWeight: '700' },
    actionBtnOutline: { backgroundColor: '#fff', borderWidth: 1.5, borderColor: '#1D4ED8' },
    actionOutlineText: { color: '#1D4ED8', fontSize: 13, fontWeight: '700' },
    diaryInput: { backgroundColor: '#F1F5F9', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', minHeight: 100, borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 12 },
    submitBtn: { backgroundColor: '#1D4ED8', padding: 14, borderRadius: 10, alignItems: 'center' },
    submitText: { color: '#fff', fontWeight: '700', fontSize: 15 },
    diaryCard: { backgroundColor: '#fff', padding: 16, borderRadius: 12, marginBottom: 10, borderLeftWidth: 4, borderLeftColor: '#3B82F6', borderWidth: 1, borderColor: '#E2E8F0' },
    dateMeta: { fontSize: 12, color: '#94A3B8', marginTop: 8 },
    emptyText: { color: '#94A3B8', textAlign: 'center', marginTop: 30 },
    emptyState: { alignItems: 'center', marginTop: 40 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155', marginTop: 12 },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, marginBottom: 20, paddingHorizontal: 32 },
});
