// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, StyleSheet, StatusBar, FlatList } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery } from '@tanstack/react-query';
import { legalApi } from '../api/legalApi';

export const LegalScreen = () => {
    const insets = useSafeAreaInsets();
    const [activeTab, setActiveTab] = useState('sections');
    const [searchQuery, setSearchQuery] = useState('');

    const sectionsQuery = useQuery({ queryKey: ['legal-sections'], queryFn: () => legalApi.getSections() });
    const sections = (sectionsQuery.data?.items || sectionsQuery.data || []).filter((s: any) =>
        !searchQuery || s.code?.includes(searchQuery) || s.title?.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const tabs = [{ key: 'sections', label: '⚖️ BNS Sections' }, { key: 'hearings', label: '🏛 Hearings' }, { key: 'bail', label: '🔓 Bail Tracker' }];

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#7C2D12" />
            <View style={s.header}>
                <Text style={s.headerTitle}>⚖️ Legal & Court Module</Text>
                <Text style={s.headerSub}>BNS/IPC sections, hearings, and bail management</Text>
            </View>

            <ScrollView horizontal showsHorizontalScrollIndicator={false} style={s.tabBar} contentContainerStyle={{ paddingHorizontal: 16 }}>
                {tabs.map(tab => (
                    <TouchableOpacity key={tab.key} onPress={() => setActiveTab(tab.key)} style={[s.tabBtn, activeTab === tab.key && s.tabBtnActive]}>
                        <Text style={[s.tabLabel, activeTab === tab.key && s.tabLabelActive]}>{tab.label}</Text>
                    </TouchableOpacity>
                ))}
            </ScrollView>

            <View style={s.body}>
                {activeTab === 'sections' && (
                    <>
                        <TextInput style={s.searchInput} placeholder="🔍 Search by section code or title..." value={searchQuery} onChangeText={setSearchQuery} placeholderTextColor="#94A3B8" />
                        {sectionsQuery.isLoading && <ActivityIndicator size="large" color="#B91C1C" style={{ marginTop: 20 }} />}
                        <FlatList
                            data={sections}
                            keyExtractor={(item: any) => item.id}
                            renderItem={({ item }) => (
                                <View style={s.sectionCard}>
                                    <View style={s.sectionTop}>
                                        <View style={s.codeBadge}><Text style={s.codeText}>§ {item.code}</Text></View>
                                        <Text style={s.sectionTitle} numberOfLines={1}>{item.title}</Text>
                                    </View>
                                    <Text style={s.actText}>{item.act}</Text>
                                    <View style={s.tagRow}>
                                        <View style={[s.tag, item.isBailable ? s.tagGreen : s.tagRed]}>
                                            <Text style={[s.tagText, item.isBailable ? s.tagGreenText : s.tagRedText]}>{item.isBailable ? '✓ Bailable' : '✕ Non-Bailable'}</Text>
                                        </View>
                                        <View style={[s.tag, item.isCognizable ? s.tagOrange : s.tagGray]}>
                                            <Text style={[s.tagText, item.isCognizable ? s.tagOrangeText : s.tagGrayText]}>{item.isCognizable ? 'Cognizable' : 'Non-Cognizable'}</Text>
                                        </View>
                                    </View>
                                    {item.maxPenalty && <Text style={s.penaltyText}>⚡ Max: {item.maxPenalty}</Text>}
                                </View>
                            )}
                            ListEmptyComponent={!sectionsQuery.isLoading ? <View style={s.emptyState}><Text style={{ fontSize: 48, marginBottom: 12 }}>📚</Text><Text style={s.emptyTitle}>No sections found</Text></View> : null}
                        />
                    </>
                )}

                {activeTab === 'hearings' && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 56, marginBottom: 16 }}>🏛</Text>
                        <Text style={s.emptyTitle}>Court Hearings</Text>
                        <Text style={s.emptyDesc}>Court hearing schedule, adjournments, and orders for cases will appear here. Hearings are tracked per case.</Text>
                    </View>
                )}

                {activeTab === 'bail' && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 56, marginBottom: 16 }}>🔓</Text>
                        <Text style={s.emptyTitle}>Bail Application Tracker</Text>
                        <Text style={s.emptyDesc}>Track bail applications, conditions, surety details, and compliance deadlines. Default bail alerts will notify you before deadlines expire.</Text>
                    </View>
                )}
            </View>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#7C2D12', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#FDBA74', marginTop: 4 },
    tabBar: { backgroundColor: '#fff', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    tabBtn: { paddingHorizontal: 14, paddingVertical: 10, borderRadius: 10, backgroundColor: '#F1F5F9', marginRight: 8 },
    tabBtnActive: { backgroundColor: '#7C2D12' },
    tabLabel: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    tabLabelActive: { color: '#fff' },
    body: { flex: 1, paddingHorizontal: 16, paddingTop: 12 },
    searchInput: { backgroundColor: '#fff', borderRadius: 10, padding: 12, fontSize: 14, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 12 },
    sectionCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    sectionTop: { flexDirection: 'row', alignItems: 'center', marginBottom: 6 },
    codeBadge: { backgroundColor: '#EEF2FF', paddingHorizontal: 10, paddingVertical: 4, borderRadius: 6, marginRight: 10 },
    codeText: { fontSize: 13, fontWeight: '800', color: '#4338CA' },
    sectionTitle: { fontSize: 15, fontWeight: '700', color: '#0F172A', flex: 1 },
    actText: { fontSize: 12, color: '#64748B', marginBottom: 8 },
    tagRow: { flexDirection: 'row', gap: 8, marginBottom: 6 },
    tag: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 6 },
    tagText: { fontSize: 11, fontWeight: '700' },
    tagGreen: { backgroundColor: '#DCFCE7' }, tagGreenText: { color: '#16A34A' },
    tagRed: { backgroundColor: '#FEE2E2' }, tagRedText: { color: '#DC2626' },
    tagOrange: { backgroundColor: '#FFF7ED' }, tagOrangeText: { color: '#C2410C' },
    tagGray: { backgroundColor: '#F1F5F9' }, tagGrayText: { color: '#64748B' },
    penaltyText: { fontSize: 12, color: '#92400E', fontWeight: '600', marginTop: 4 },
    emptyState: { alignItems: 'center', marginTop: 60 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155' },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 32 },
});
