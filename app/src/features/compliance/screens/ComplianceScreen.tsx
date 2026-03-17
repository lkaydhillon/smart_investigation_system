// @ts-nocheck
import React from 'react';
import { View, Text, ScrollView, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

const ALERTS = [
    { id: 1, type: 'Default Bail', severity: 'critical', section: 'Section 167(2) CrPC', case: 'FIR-2026-00147', daysLeft: 3, totalDays: 90, description: 'If chargesheet not filed within 90 days, accused entitled to default bail.' },
    { id: 2, type: 'POCSO Timeline', severity: 'high', section: 'POCSO Act', case: 'FIR-2026-00082', daysLeft: 12, totalDays: 60, description: 'POCSO cases require investigation completion and chargesheet within 60 days.' },
    { id: 3, type: 'Investigation Deadline', severity: 'medium', section: 'Section 173 CrPC', case: 'FIR-2026-00201', daysLeft: 45, totalDays: 180, description: 'Standard investigation deadline for serious offences (6 months).' },
    { id: 4, type: 'Remand Expiry', severity: 'high', section: 'Section 167 CrPC', case: 'FIR-2026-00147', daysLeft: 5, totalDays: 15, description: 'Police remand (15 days max). Apply for judicial remand or release accused.' },
    { id: 5, type: 'Court Hearing', severity: 'medium', section: 'Next hearing', case: 'FIR-2026-00082', daysLeft: 7, totalDays: 30, description: 'Next court hearing scheduled. Prepare prosecution documents.' },
];

export const ComplianceScreen = () => {
    const insets = useSafeAreaInsets();

    const getSeverityStyle = (sev: string) => {
        if (sev === 'critical') return { bg: '#FEE2E2', border: '#FECACA', text: '#DC2626', icon: '🔴', label: 'CRITICAL' };
        if (sev === 'high') return { bg: '#FFF7ED', border: '#FED7AA', text: '#C2410C', icon: '🟠', label: 'HIGH' };
        return { bg: '#FEF9C3', border: '#FDE68A', text: '#A16207', icon: '🟡', label: 'MEDIUM' };
    };

    const criticalCount = ALERTS.filter(a => a.severity === 'critical').length;
    const highCount = ALERTS.filter(a => a.severity === 'high').length;

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#991B1B" />
            <View style={s.header}>
                <Text style={s.headerTitle}>⏰ Compliance Monitor</Text>
                <Text style={s.headerSub}>Legal deadline alerts & procedural compliance</Text>
                <View style={s.alertSummary}>
                    <View style={s.summaryItem}><Text style={s.summaryNum}>{criticalCount}</Text><Text style={s.summaryLabel}>Critical</Text></View>
                    <View style={[s.summaryItem, { borderLeftWidth: 1, borderLeftColor: 'rgba(255,255,255,0.2)' }]}><Text style={[s.summaryNum, { color: '#FDBA74' }]}>{highCount}</Text><Text style={s.summaryLabel}>High</Text></View>
                    <View style={[s.summaryItem, { borderLeftWidth: 1, borderLeftColor: 'rgba(255,255,255,0.2)' }]}><Text style={[s.summaryNum, { color: '#FDE68A' }]}>{ALERTS.length - criticalCount - highCount}</Text><Text style={s.summaryLabel}>Medium</Text></View>
                </View>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {ALERTS.sort((a, b) => a.daysLeft - b.daysLeft).map(alert => {
                    const sv = getSeverityStyle(alert.severity);
                    const progress = ((alert.totalDays - alert.daysLeft) / alert.totalDays) * 100;
                    return (
                        <View key={alert.id} style={[s.alertCard, { borderLeftColor: sv.text }]}>
                            <View style={s.alertTop}>
                                <View style={[s.sevBadge, { backgroundColor: sv.bg }]}><Text style={{ fontSize: 11, fontWeight: '700', color: sv.text }}>{sv.icon} {sv.label}</Text></View>
                                <Text style={s.daysLeft}>{alert.daysLeft} days left</Text>
                            </View>
                            <Text style={s.alertType}>{alert.type}</Text>
                            <Text style={s.alertCase}>{alert.case} • {alert.section}</Text>
                            <Text style={s.alertDesc}>{alert.description}</Text>
                            {/* Progress bar */}
                            <View style={s.progressBg}>
                                <View style={[s.progressFill, { width: `${Math.min(progress, 100)}%`, backgroundColor: progress > 80 ? '#DC2626' : progress > 50 ? '#F59E0B' : '#16A34A' }]} />
                            </View>
                            <Text style={s.progressText}>{Math.round(progress)}% of time elapsed</Text>
                        </View>
                    );
                })}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#991B1B', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#FCA5A5', marginTop: 4 },
    alertSummary: { flexDirection: 'row', marginTop: 14, backgroundColor: 'rgba(0,0,0,0.2)', borderRadius: 12, padding: 12 },
    summaryItem: { flex: 1, alignItems: 'center' },
    summaryNum: { fontSize: 24, fontWeight: '800', color: '#FCA5A5' },
    summaryLabel: { fontSize: 11, color: 'rgba(255,255,255,0.6)', marginTop: 2 },
    body: { flex: 1, padding: 16 },
    alertCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', borderLeftWidth: 4, elevation: 1 },
    alertTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
    sevBadge: { paddingHorizontal: 10, paddingVertical: 3, borderRadius: 6 },
    daysLeft: { fontSize: 14, fontWeight: '800', color: '#0F172A' },
    alertType: { fontSize: 16, fontWeight: '700', color: '#0F172A', marginBottom: 2 },
    alertCase: { fontSize: 12, color: '#64748B', marginBottom: 6 },
    alertDesc: { fontSize: 13, color: '#475569', lineHeight: 20, marginBottom: 10 },
    progressBg: { height: 6, backgroundColor: '#F1F5F9', borderRadius: 3, overflow: 'hidden' },
    progressFill: { height: 6, borderRadius: 3 },
    progressText: { fontSize: 11, color: '#94A3B8', marginTop: 4, textAlign: 'right' },
});
