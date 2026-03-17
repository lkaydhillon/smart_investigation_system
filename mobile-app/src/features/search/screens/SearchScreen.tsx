// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery } from '@tanstack/react-query';
import { apiClient } from '../../../core/api';

export const SearchScreen = () => {
    const insets = useSafeAreaInsets();
    const [mode, setMode] = useState('fuzzy');
    const [query, setQuery] = useState('');
    const [results, setResults] = useState<any[]>([]);
    const [searched, setSearched] = useState(false);

    const complaintsQuery = useQuery({ queryKey: ['search-complaints'], queryFn: async () => { const r = await apiClient.get('/Complaints?pageSize=100'); return r.data; }, enabled: false });
    const casesQuery = useQuery({ queryKey: ['search-cases'], queryFn: async () => { const r = await apiClient.get('/Cases?pageSize=100'); return r.data; }, enabled: false });

    const handleFuzzySearch = async () => {
        if (!query.trim()) return;
        setSearched(true);
        try {
            const [compRes, caseRes] = await Promise.all([apiClient.get('/Complaints?pageSize=100'), apiClient.get('/Cases?pageSize=100')]);
            const q = query.toLowerCase();
            const matchedComplaints = (compRes.data.items || []).filter((c: any) =>
                c.complainantName?.toLowerCase().includes(q) || c.description?.toLowerCase().includes(q) || c.complaintNumber?.toLowerCase().includes(q)
            ).map((c: any) => ({ ...c, _type: 'complaint' }));
            const matchedCases = (caseRes.data.items || []).filter((c: any) =>
                c.caseNumber?.toLowerCase().includes(q) || c.title?.toLowerCase().includes(q) || c.description?.toLowerCase().includes(q)
            ).map((c: any) => ({ ...c, _type: 'case' }));
            setResults([...matchedCases, ...matchedComplaints]);
        } catch { setResults([]); }
    };

    const handleNLSearch = async () => {
        if (!query.trim()) return;
        setSearched(true);
        try {
            const caseRes = await apiClient.get('/Cases?pageSize=100');
            const items = caseRes.data.items || [];
            const q = query.toLowerCase();
            let filtered = items;
            if (q.includes('heinous') || q.includes('critical') || q.includes('murder')) filtered = items.filter((c: any) => c.priority === 'Critical' || c.priority === 'High');
            if (q.includes('pending') || q.includes('open')) filtered = filtered.filter((c: any) => c.status !== 'Closed');
            if (q.includes('theft')) filtered = items.filter((c: any) => c.title?.toLowerCase().includes('theft') || c.description?.toLowerCase().includes('theft'));
            setResults(filtered.map((c: any) => ({ ...c, _type: 'case' })));
        } catch { setResults([]); }
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#0F766E" />
            <View style={s.header}>
                <Text style={s.headerTitle}>🔎 Advanced Search</Text>
                <View style={s.modeRow}>
                    <TouchableOpacity onPress={() => setMode('fuzzy')} style={[s.modeBtn, mode === 'fuzzy' && s.modeBtnActive]}>
                        <Text style={[s.modeText, mode === 'fuzzy' && s.modeTextActive]}>Fuzzy Match</Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={() => setMode('nl')} style={[s.modeBtn, mode === 'nl' && s.modeBtnActive]}>
                        <Text style={[s.modeText, mode === 'nl' && s.modeTextActive]}>Natural Language</Text>
                    </TouchableOpacity>
                </View>
                <View style={s.searchRow}>
                    <TextInput style={s.searchInput} placeholder={mode === 'fuzzy' ? 'Search by name, number, description...' : 'E.g., Show all pending heinous cases...'} value={query} onChangeText={setQuery} placeholderTextColor="rgba(255,255,255,0.5)" onSubmitEditing={mode === 'fuzzy' ? handleFuzzySearch : handleNLSearch} returnKeyType="search" />
                    <TouchableOpacity onPress={mode === 'fuzzy' ? handleFuzzySearch : handleNLSearch} style={s.searchBtn}><Text style={s.searchBtnText}>Go</Text></TouchableOpacity>
                </View>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {searched && <Text style={s.resultCount}>{results.length} result{results.length !== 1 ? 's' : ''} found</Text>}
                {results.map((item, i) => (
                    <View key={item.id || i} style={s.resultCard}>
                        <View style={s.resultTop}>
                            <View style={[s.typeBadge, item._type === 'case' ? s.caseBadge : s.compBadge]}>
                                <Text style={[s.typeText, item._type === 'case' ? s.caseText : s.compText]}>{item._type === 'case' ? '📁 Case' : '📝 Complaint'}</Text>
                            </View>
                            {item.priority && <View style={[s.priBadge, { backgroundColor: item.priority === 'Critical' ? '#FEE2E2' : item.priority === 'High' ? '#FFF7ED' : '#F0FDF4' }]}>
                                <Text style={{ fontSize: 10, fontWeight: '700', color: item.priority === 'Critical' ? '#DC2626' : item.priority === 'High' ? '#EA580C' : '#16A34A' }}>{item.priority}</Text>
                            </View>}
                        </View>
                        <Text style={s.resultTitle}>{item.caseNumber || item.complaintNumber}</Text>
                        <Text style={s.resultDesc} numberOfLines={2}>{item.title || item.description}</Text>
                        {item.complainantName && <Text style={s.resultMeta}>👤 {item.complainantName}</Text>}
                    </View>
                ))}
                {searched && results.length === 0 && (
                    <View style={s.emptyState}><Text style={{ fontSize: 48 }}>🔍</Text><Text style={s.emptyTitle}>No results found</Text><Text style={s.emptyDesc}>Try different keywords or switch to {mode === 'fuzzy' ? 'Natural Language' : 'Fuzzy'} mode.</Text></View>
                )}
                {!searched && (
                    <View style={s.emptyState}><Text style={{ fontSize: 56, marginBottom: 12 }}>🔎</Text><Text style={s.emptyTitle}>Search Cases & Complaints</Text><Text style={s.emptyDesc}>{mode === 'fuzzy' ? 'Search by name, FIR number, or description with fuzzy matching for spelling variations.' : 'Use natural language queries like "Show all pending heinous cases" or "Cases with theft in last week".'}</Text></View>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#0F766E', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff', marginBottom: 12 },
    modeRow: { flexDirection: 'row', marginBottom: 12, gap: 8 },
    modeBtn: { flex: 1, paddingVertical: 10, borderRadius: 8, backgroundColor: 'rgba(255,255,255,0.15)', alignItems: 'center' },
    modeBtnActive: { backgroundColor: '#fff' },
    modeText: { fontSize: 13, fontWeight: '600', color: 'rgba(255,255,255,0.7)' },
    modeTextActive: { color: '#0F766E' },
    searchRow: { flexDirection: 'row', gap: 8 },
    searchInput: { flex: 1, backgroundColor: 'rgba(255,255,255,0.15)', borderRadius: 10, padding: 12, fontSize: 14, color: '#fff' },
    searchBtn: { backgroundColor: '#fff', paddingHorizontal: 20, borderRadius: 10, justifyContent: 'center' },
    searchBtnText: { color: '#0F766E', fontWeight: '700', fontSize: 14 },
    body: { flex: 1, padding: 16 },
    resultCount: { fontSize: 13, color: '#64748B', fontWeight: '600', marginBottom: 12 },
    resultCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    resultTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
    typeBadge: { paddingHorizontal: 10, paddingVertical: 3, borderRadius: 6 },
    caseBadge: { backgroundColor: '#EFF6FF' }, caseText: { color: '#1D4ED8' },
    compBadge: { backgroundColor: '#FFF7ED' }, compText: { color: '#C2410C' },
    typeText: { fontSize: 11, fontWeight: '700' },
    priBadge: { paddingHorizontal: 8, paddingVertical: 2, borderRadius: 4 },
    resultTitle: { fontSize: 15, fontWeight: '700', color: '#0F172A', marginBottom: 4 },
    resultDesc: { fontSize: 13, color: '#64748B', lineHeight: 20 },
    resultMeta: { fontSize: 12, color: '#94A3B8', marginTop: 6 },
    emptyState: { alignItems: 'center', marginTop: 60 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155', marginTop: 8 },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 24 },
});
