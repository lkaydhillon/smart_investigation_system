// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, StyleSheet, StatusBar, Alert, FlatList } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { investigationApi } from '../api/investigationApi';

export const InvestigationScreen = () => {
    const insets = useSafeAreaInsets();
    const queryClient = useQueryClient();
    const [caseId, setCaseId] = useState('');
    const [loaded, setLoaded] = useState(false);
    const [activeTab, setActiveTab] = useState('steps');
    const [diaryEntry, setDiaryEntry] = useState('');

    const stepsQuery = useQuery({ queryKey: ['inv-steps', caseId], queryFn: () => investigationApi.getSteps(caseId), enabled: loaded });
    const diaryQuery = useQuery({ queryKey: ['inv-diary', caseId], queryFn: () => investigationApi.getDiary(caseId), enabled: loaded && activeTab === 'diary' });
    const interrogationQuery = useQuery({ queryKey: ['inv-interr', caseId], queryFn: () => investigationApi.getInterrogations(caseId), enabled: loaded && activeTab === 'interrogations' });

    const addDiaryMutation = useMutation({
        mutationFn: () => investigationApi.addDiaryEntry(caseId, { content: diaryEntry, entryDate: new Date().toISOString() }),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inv-diary'] }); setDiaryEntry(''); Alert.alert('✅ Added', 'Diary entry recorded'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to add entry'),
    });

    const handleLoadCase = () => {
        if (!caseId.trim()) { Alert.alert('Required', 'Please enter a Case ID'); return; }
        setLoaded(true);
    };

    const steps = stepsQuery.data?.items || stepsQuery.data || [];
    const diaryEntries = diaryQuery.data?.items || diaryQuery.data || [];
    const interrogations = interrogationQuery.data?.items || interrogationQuery.data || [];

    const tabs = [{ key: 'steps', label: '📋 SOP Steps' }, { key: 'diary', label: '📖 Case Diary' }, { key: 'interrogations', label: '❓ Interrogations' }];

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#1E3A5F" />
            <View style={s.header}>
                <Text style={s.headerTitle}>🔍 Investigation Workflow</Text>
                <View style={s.searchRow}>
                    <TextInput style={s.searchInput} placeholder="Enter Case ID..." value={caseId} onChangeText={t => { setCaseId(t); setLoaded(false); }} placeholderTextColor="#94A3B8" />
                    <TouchableOpacity onPress={handleLoadCase} style={s.loadBtn}><Text style={s.loadBtnText}>Load</Text></TouchableOpacity>
                </View>
            </View>

            {loaded && (
                <ScrollView horizontal showsHorizontalScrollIndicator={false} style={s.tabBar} contentContainerStyle={{ paddingHorizontal: 16 }}>
                    {tabs.map(tab => (
                        <TouchableOpacity key={tab.key} onPress={() => setActiveTab(tab.key)} style={[s.tabBtn, activeTab === tab.key && s.tabBtnActive]}>
                            <Text style={[s.tabLabel, activeTab === tab.key && s.tabLabelActive]}>{tab.label}</Text>
                        </TouchableOpacity>
                    ))}
                </ScrollView>
            )}

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {!loaded && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 56, marginBottom: 16 }}>🔍</Text>
                        <Text style={s.emptyTitle}>Investigation Workflow</Text>
                        <Text style={s.emptyDesc}>Enter a Case ID above to load SOP steps, case diary entries, and interrogation records.</Text>
                    </View>
                )}

                {/* SOP STEPS */}
                {loaded && activeTab === 'steps' && (
                    <>
                        {stepsQuery.isLoading && <ActivityIndicator size="large" color="#1D4ED8" style={{ marginTop: 20 }} />}
                        {steps.length === 0 && !stepsQuery.isLoading && (
                            <View style={s.infoCard}>
                                <Text style={{ fontSize: 24, marginBottom: 8 }}>📋</Text>
                                <Text style={s.infoText}>No SOP steps configured for this case yet.</Text>
                            </View>
                        )}
                        {steps.map((step: any, i: number) => (
                            <View key={step.id || i} style={s.stepCard}>
                                <View style={[s.stepNumber, step.status === 'Completed' ? s.stepDone : step.status === 'InProgress' ? s.stepProgress : s.stepPending]}>
                                    <Text style={s.stepNumText}>{step.status === 'Completed' ? '✓' : step.stepOrder || i + 1}</Text>
                                </View>
                                <View style={{ flex: 1 }}>
                                    <Text style={s.stepTitle}>{step.stepTitle || step.title || `Step ${step.stepOrder || i + 1}`}</Text>
                                    <Text style={s.stepMeta}>{step.assignedToName || 'Unassigned'} • {step.status || 'Pending'}</Text>
                                </View>
                                {step.isMandatory && <View style={s.mandatoryBadge}><Text style={s.mandatoryText}>Required</Text></View>}
                            </View>
                        ))}
                    </>
                )}

                {/* CASE DIARY */}
                {loaded && activeTab === 'diary' && (
                    <>
                        <View style={s.diaryForm}>
                            <TextInput style={s.diaryInput} placeholder="Write case diary entry..." multiline numberOfLines={4} textAlignVertical="top" value={diaryEntry} onChangeText={setDiaryEntry} placeholderTextColor="#94A3B8" />
                            <TouchableOpacity onPress={() => addDiaryMutation.mutate()} disabled={!diaryEntry.trim() || addDiaryMutation.isPending} style={[s.addBtn, !diaryEntry.trim() && s.btnDisabled]}>
                                {addDiaryMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.addBtnText}>📝 Add Entry</Text>}
                            </TouchableOpacity>
                        </View>
                        {diaryQuery.isLoading && <ActivityIndicator size="large" color="#1D4ED8" />}
                        {diaryEntries.map((entry: any, i: number) => (
                            <View key={entry.id || i} style={s.diaryCard}>
                                <Text style={s.diaryContent}>{entry.content}</Text>
                                <Text style={s.diaryMeta}>{entry.officerName || 'IO'} • {new Date(entry.entryDate || entry.createdDate || Date.now()).toLocaleDateString('en-IN')}</Text>
                            </View>
                        ))}
                        {!diaryQuery.isLoading && diaryEntries.length === 0 && <Text style={s.emptyText}>No diary entries recorded yet.</Text>}
                    </>
                )}

                {/* INTERROGATIONS */}
                {loaded && activeTab === 'interrogations' && (
                    <>
                        {interrogationQuery.isLoading && <ActivityIndicator size="large" color="#1D4ED8" />}
                        {interrogations.map((q: any, i: number) => (
                            <View key={q.id || i} style={s.interrogCard}>
                                <View style={s.interrogHeader}>
                                    <Text style={s.interrogName}>{q.personName || 'Subject'}</Text>
                                    <Text style={s.interrogDate}>{new Date(q.dateTime || Date.now()).toLocaleDateString('en-IN')}</Text>
                                </View>
                                <Text style={s.interrogMeta}>By: {q.interrogatorName || 'IO'} • {q.location || 'Station'}</Text>
                            </View>
                        ))}
                        {!interrogationQuery.isLoading && interrogations.length === 0 && <Text style={s.emptyText}>No interrogations recorded.</Text>}
                    </>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#1E3A5F', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff', marginBottom: 12 },
    searchRow: { flexDirection: 'row', gap: 8 },
    searchInput: { flex: 1, backgroundColor: 'rgba(255,255,255,0.15)', borderRadius: 10, padding: 12, fontSize: 14, color: '#fff' },
    loadBtn: { backgroundColor: '#3B82F6', paddingHorizontal: 20, borderRadius: 10, justifyContent: 'center' },
    loadBtnText: { color: '#fff', fontWeight: '700', fontSize: 14 },
    tabBar: { backgroundColor: '#fff', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    tabBtn: { paddingHorizontal: 14, paddingVertical: 10, borderRadius: 10, backgroundColor: '#F1F5F9', marginRight: 8 },
    tabBtnActive: { backgroundColor: '#1E3A5F' },
    tabLabel: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    tabLabelActive: { color: '#fff' },
    body: { flex: 1, padding: 16 },
    emptyState: { alignItems: 'center', marginTop: 60 },
    emptyTitle: { fontSize: 20, fontWeight: '700', color: '#334155' },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 32 },
    emptyText: { color: '#94A3B8', textAlign: 'center', marginTop: 30 },
    infoCard: { backgroundColor: '#EFF6FF', borderRadius: 14, padding: 20, alignItems: 'center', marginTop: 16 },
    infoText: { fontSize: 14, color: '#1E40AF', textAlign: 'center' },
    stepCard: { backgroundColor: '#fff', borderRadius: 14, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0', flexDirection: 'row', alignItems: 'center' },
    stepNumber: { width: 40, height: 40, borderRadius: 12, justifyContent: 'center', alignItems: 'center', marginRight: 14 },
    stepDone: { backgroundColor: '#DCFCE7' }, stepProgress: { backgroundColor: '#FEF9C3' }, stepPending: { backgroundColor: '#F1F5F9' },
    stepNumText: { fontSize: 14, fontWeight: '800', color: '#334155' },
    stepTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A' },
    stepMeta: { fontSize: 12, color: '#94A3B8', marginTop: 2 },
    mandatoryBadge: { backgroundColor: '#FEE2E2', paddingHorizontal: 8, paddingVertical: 3, borderRadius: 6 },
    mandatoryText: { fontSize: 10, fontWeight: '700', color: '#DC2626' },
    diaryForm: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0' },
    diaryInput: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', minHeight: 80, borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 12 },
    addBtn: { backgroundColor: '#1E3A5F', padding: 14, borderRadius: 10, alignItems: 'center' },
    addBtnText: { color: '#fff', fontWeight: '700', fontSize: 14 },
    btnDisabled: { backgroundColor: '#CBD5E1' },
    diaryCard: { backgroundColor: '#fff', padding: 16, borderRadius: 12, marginBottom: 8, borderLeftWidth: 4, borderLeftColor: '#3B82F6', borderWidth: 1, borderColor: '#E2E8F0' },
    diaryContent: { fontSize: 14, color: '#334155', lineHeight: 22 },
    diaryMeta: { fontSize: 12, color: '#94A3B8', marginTop: 8 },
    interrogCard: { backgroundColor: '#fff', borderRadius: 12, padding: 16, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0' },
    interrogHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 4 },
    interrogName: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    interrogDate: { fontSize: 12, color: '#94A3B8' },
    interrogMeta: { fontSize: 13, color: '#64748B' },
});
