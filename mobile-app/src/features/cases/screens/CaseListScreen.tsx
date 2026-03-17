// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, FlatList, TouchableOpacity, ActivityIndicator, TextInput, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery } from '@tanstack/react-query';
import { casesApi } from '../api/casesApi';
import { useNavigation } from '@react-navigation/native';

export const CaseListScreen = () => {
    const insets = useSafeAreaInsets();
    const [searchQuery, setSearchQuery] = useState('');
    const navigation = useNavigation();

    const { data, isLoading, isError, refetch } = useQuery({
        queryKey: ['cases', 'list'],
        queryFn: () => casesApi.getDashboardSummary(),
    });

    const cases = data?.items || [];
    const filtered = cases.filter((c: any) => {
        const q = searchQuery.toLowerCase();
        return !q || (c.caseNumber?.toLowerCase().includes(q) || c.title?.toLowerCase().includes(q) || c.status?.toLowerCase().includes(q));
    });

    const getPriorityColor = (p: string) => {
        if (p === 'Critical') return { bg: '#FEE2E2', text: '#DC2626' };
        if (p === 'High') return { bg: '#FFF7ED', text: '#EA580C' };
        if (p === 'Medium') return { bg: '#FEF9C3', text: '#A16207' };
        return { bg: '#F0FDF4', text: '#16A34A' };
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="dark-content" backgroundColor="#fff" />
            <View style={s.header}>
                <View style={s.headerRow}>
                    <Text style={s.title}>Case Registry</Text>
                    <View style={s.countBadge}><Text style={s.countText}>{cases.length} cases</Text></View>
                </View>
                <TextInput style={s.searchInput} placeholder="🔍 Search by number, title, status..." value={searchQuery} onChangeText={setSearchQuery} placeholderTextColor="#94A3B8" />
            </View>

            {isLoading && <ActivityIndicator size="large" color="#1D4ED8" style={{ marginTop: 40 }} />}

            {isError && (
                <View style={{ alignItems: 'center', marginTop: 40 }}>
                    <Text style={{ color: '#DC2626', marginBottom: 12 }}>Failed to load. Check backend.</Text>
                    <TouchableOpacity onPress={() => refetch()} style={s.retryBtn}><Text style={{ color: '#fff', fontWeight: '700' }}>Retry</Text></TouchableOpacity>
                </View>
            )}

            <FlatList
                data={filtered}
                keyExtractor={item => item.id}
                contentContainerStyle={{ padding: 16 }}
                renderItem={({ item }) => {
                    const pc = getPriorityColor(item.priority);
                    return (
                        <TouchableOpacity onPress={() => navigation.navigate('CaseDetail', { caseId: item.id })} style={s.card}>
                            <View style={s.cardTop}>
                                <Text style={s.caseNumber}>{item.caseNumber}</Text>
                                <View style={[s.priorityBadge, { backgroundColor: pc.bg }]}>
                                    <Text style={{ fontSize: 11, fontWeight: '700', color: pc.text }}>{item.priority}</Text>
                                </View>
                            </View>
                            <Text style={s.caseTitle} numberOfLines={2}>{item.title || item.description || 'No title'}</Text>
                            <View style={s.cardBottom}>
                                <View style={s.statusRow}>
                                    <View style={[s.dot, { backgroundColor: item.status === 'Registered' ? '#3B82F6' : item.status === 'UnderInvestigation' ? '#F59E0B' : '#16A34A' }]} />
                                    <Text style={s.statusText}>{item.status}</Text>
                                </View>
                                <Text style={s.dateText}>{new Date(item.createdDate).toLocaleDateString('en-IN')}</Text>
                            </View>
                            {item.isHighProfile && <View style={s.hpBadge}><Text style={s.hpText}>⭐ HIGH PROFILE</Text></View>}
                        </TouchableOpacity>
                    );
                }}
                ListEmptyComponent={!isLoading ? <Text style={{ color: '#94A3B8', textAlign: 'center', marginTop: 40 }}>No cases found</Text> : null}
                onRefresh={() => refetch()}
                refreshing={isLoading}
            />
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#fff', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    headerRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 },
    title: { fontSize: 24, fontWeight: '800', color: '#0F172A' },
    countBadge: { backgroundColor: '#EFF6FF', paddingHorizontal: 12, paddingVertical: 4, borderRadius: 8 },
    countText: { fontSize: 13, fontWeight: '600', color: '#1D4ED8' },
    searchInput: { backgroundColor: '#F1F5F9', borderRadius: 10, padding: 12, fontSize: 14, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0' },
    retryBtn: { backgroundColor: '#1D4ED8', paddingHorizontal: 20, paddingVertical: 10, borderRadius: 8 },
    card: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1, shadowColor: '#000', shadowOpacity: 0.03, shadowRadius: 4, shadowOffset: { width: 0, height: 1 } },
    cardTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
    caseNumber: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    caseTitle: { fontSize: 14, color: '#475569', lineHeight: 20, marginBottom: 10 },
    cardBottom: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
    statusRow: { flexDirection: 'row', alignItems: 'center' },
    dot: { width: 8, height: 8, borderRadius: 4, marginRight: 6 },
    statusText: { fontSize: 12, color: '#64748B', fontWeight: '500' },
    dateText: { fontSize: 12, color: '#94A3B8' },
    priorityBadge: { paddingHorizontal: 10, paddingVertical: 3, borderRadius: 8 },
    hpBadge: { backgroundColor: '#FEF9C3', paddingHorizontal: 10, paddingVertical: 4, borderRadius: 6, marginTop: 8, alignSelf: 'flex-start' },
    hpText: { fontSize: 11, fontWeight: '700', color: '#A16207' },
});
