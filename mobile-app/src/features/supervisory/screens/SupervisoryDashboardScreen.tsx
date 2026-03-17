// @ts-nocheck
import React from 'react';
import { View, Text, ScrollView, StyleSheet, StatusBar, Dimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

const { width } = Dimensions.get('window');

const METRICS = {
    totalCases: 48, pendingCases: 12, closedCases: 31, chargeSheetFiled: 28,
    clearanceRate: 64.6, pendencyRate: 25.0, avgDays: 42, convictionRate: 38.5,
};

const OFFICERS = [
    { name: 'SI Anjali Mehta', badge: 'IO-2024-001', cases: 8, closed: 5, pending: 3 },
    { name: 'ASI Ravi Teja', badge: 'IO-2024-002', cases: 6, closed: 4, pending: 2 },
    { name: 'Inspector Vikram', badge: 'SHO-2024-001', cases: 15, closed: 10, pending: 5 },
];

export const SupervisoryDashboardScreen = () => {
    const insets = useSafeAreaInsets();

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#1E3A5F" />
            <View style={s.header}>
                <Text style={s.headerTitle}>📈 Supervisory Dashboard</Text>
                <Text style={s.headerSub}>Case pendency, performance metrics & officer analytics</Text>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* Key Metrics */}
                <Text style={s.sectionTitle}>Key Performance Indicators</Text>
                <View style={s.metricsGrid}>
                    {[
                        { label: 'Total Cases', value: METRICS.totalCases, icon: '📁', bg: '#EFF6FF' },
                        { label: 'Pending', value: METRICS.pendingCases, icon: '⏳', bg: '#FEF9C3' },
                        { label: 'Closed', value: METRICS.closedCases, icon: '✅', bg: '#DCFCE7' },
                        { label: 'Chargesheets', value: METRICS.chargeSheetFiled, icon: '📋', bg: '#EDE9FE' },
                    ].map(m => (
                        <View key={m.label} style={[s.metricCard, { backgroundColor: m.bg }]}>
                            <Text style={{ fontSize: 20 }}>{m.icon}</Text>
                            <Text style={s.metricValue}>{m.value}</Text>
                            <Text style={s.metricLabel}>{m.label}</Text>
                        </View>
                    ))}
                </View>

                {/* Performance Rates */}
                <Text style={s.sectionTitle}>Performance Rates</Text>
                {[
                    { label: 'Clearance Rate', value: METRICS.clearanceRate, color: '#16A34A' },
                    { label: 'Pendency Rate', value: METRICS.pendencyRate, color: '#DC2626' },
                    { label: 'Conviction Rate', value: METRICS.convictionRate, color: '#2563EB' },
                ].map(r => (
                    <View key={r.label} style={s.rateCard}>
                        <View style={s.rateTop}>
                            <Text style={s.rateLabel}>{r.label}</Text>
                            <Text style={[s.rateValue, { color: r.color }]}>{r.value}%</Text>
                        </View>
                        <View style={s.progressBg}>
                            <View style={[s.progressFill, { width: `${r.value}%`, backgroundColor: r.color }]} />
                        </View>
                    </View>
                ))}

                <View style={s.avgCard}>
                    <Text style={{ fontSize: 18 }}>⏱️</Text>
                    <Text style={s.avgLabel}>Avg. Investigation Duration</Text>
                    <Text style={s.avgValue}>{METRICS.avgDays} days</Text>
                </View>

                {/* Officer Performance */}
                <Text style={s.sectionTitle}>Officer Performance</Text>
                {OFFICERS.map(o => (
                    <View key={o.badge} style={s.officerCard}>
                        <View style={s.avatar}><Text style={s.avatarText}>{o.name[0]}</Text></View>
                        <View style={{ flex: 1 }}>
                            <Text style={s.officerName}>{o.name}</Text>
                            <Text style={s.officerBadge}>{o.badge}</Text>
                        </View>
                        <View style={s.officerStats}>
                            <Text style={s.officerStatNum}>{o.cases}</Text>
                            <Text style={s.officerStatLabel}>cases</Text>
                        </View>
                        <View style={s.officerStats}>
                            <Text style={[s.officerStatNum, { color: '#16A34A' }]}>{o.closed}</Text>
                            <Text style={s.officerStatLabel}>closed</Text>
                        </View>
                        <View style={s.officerStats}>
                            <Text style={[s.officerStatNum, { color: '#DC2626' }]}>{o.pending}</Text>
                            <Text style={s.officerStatLabel}>pending</Text>
                        </View>
                    </View>
                ))}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#1E3A5F', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#93C5FD', marginTop: 4 },
    body: { flex: 1, padding: 16 },
    sectionTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 12, marginTop: 8 },
    metricsGrid: { flexDirection: 'row', flexWrap: 'wrap', gap: 10, marginBottom: 16 },
    metricCard: { width: (width - 42) / 2, borderRadius: 14, padding: 16, alignItems: 'center' },
    metricValue: { fontSize: 28, fontWeight: '800', color: '#0F172A', marginTop: 4 },
    metricLabel: { fontSize: 12, color: '#64748B', marginTop: 2 },
    rateCard: { backgroundColor: '#fff', borderRadius: 12, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0' },
    rateTop: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 8 },
    rateLabel: { fontSize: 14, fontWeight: '600', color: '#334155' },
    rateValue: { fontSize: 18, fontWeight: '800' },
    progressBg: { height: 8, backgroundColor: '#F1F5F9', borderRadius: 4, overflow: 'hidden' },
    progressFill: { height: 8, borderRadius: 4 },
    avgCard: { backgroundColor: '#FFF7ED', borderRadius: 14, padding: 16, flexDirection: 'row', alignItems: 'center', gap: 12, marginTop: 8, marginBottom: 8 },
    avgLabel: { fontSize: 13, color: '#92400E', flex: 1 },
    avgValue: { fontSize: 20, fontWeight: '800', color: '#C2410C' },
    officerCard: { backgroundColor: '#fff', borderRadius: 14, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0', flexDirection: 'row', alignItems: 'center' },
    avatar: { width: 40, height: 40, borderRadius: 12, backgroundColor: '#EFF6FF', justifyContent: 'center', alignItems: 'center', marginRight: 12 },
    avatarText: { fontSize: 16, fontWeight: '800', color: '#1D4ED8' },
    officerName: { fontSize: 14, fontWeight: '700', color: '#0F172A' },
    officerBadge: { fontSize: 11, color: '#94A3B8' },
    officerStats: { alignItems: 'center', marginLeft: 14 },
    officerStatNum: { fontSize: 16, fontWeight: '800', color: '#0F172A' },
    officerStatLabel: { fontSize: 10, color: '#94A3B8' },
});
