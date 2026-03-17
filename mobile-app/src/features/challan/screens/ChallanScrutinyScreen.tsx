// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, StyleSheet, StatusBar, Alert } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

const SECTIONS_ANALYSIS = [
    { section: '§103 BNS', title: 'Murder', status: 'verified', finding: 'Sufficient evidence - eyewitness and forensic confirmation', icon: '✅' },
    { section: '§115 BNS', title: 'Voluntary causing hurt', status: 'weak', finding: 'Medical evidence present but witness corroboration is missing', icon: '⚠️' },
    { section: '§351 BNS', title: 'Criminal intimidation', status: 'verified', finding: 'Corroborated by statement of complainant and phone records', icon: '✅' },
];

const CHECKLIST = [
    { item: 'Accused identification by witness', done: true },
    { item: 'Scene of crime survey report', done: true },
    { item: 'Forensic lab report (fingerprints, DNA)', done: true },
    { item: 'Phone/CDR records attached', done: false },
    { item: 'Motive established in investigation', done: true },
    { item: 'All witness statements recorded u/s 161', done: false },
    { item: 'Confessional statement u/s 164', done: false },
    { item: 'TIP (Test Identification Parade)', done: false },
    { item: 'Sanction / Court orders if required', done: true },
];

export const ChallanScrutinyScreen = () => {
    const insets = useSafeAreaInsets();
    const completedCount = CHECKLIST.filter(c => c.done).length;
    const score = Math.round((completedCount / CHECKLIST.length) * 100);

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#92400E" />
            <View style={s.header}>
                <Text style={s.headerTitle}>📋 Challan Scrutiny</Text>
                <Text style={s.headerSub}>AI-assisted chargesheet quality review</Text>
                <View style={s.scoreRow}>
                    <View style={s.scoreCircle}>
                        <Text style={s.scoreText}>{score}%</Text>
                    </View>
                    <View>
                        <Text style={s.scoreLabel}>Chargesheet Readiness</Text>
                        <Text style={s.scoreMeta}>{completedCount}/{CHECKLIST.length} items verified</Text>
                    </View>
                </View>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* Section-wise Analysis */}
                <Text style={s.sectionTitle}>Section-wise Evidence Analysis</Text>
                {SECTIONS_ANALYSIS.map((sec, i) => (
                    <View key={i} style={[s.sectionCard, { borderLeftColor: sec.status === 'verified' ? '#16A34A' : '#F59E0B' }]}>
                        <View style={s.sectionTop}>
                            <View style={s.codeBadge}><Text style={s.codeText}>{sec.section}</Text></View>
                            <Text style={{ fontSize: 16 }}>{sec.icon}</Text>
                        </View>
                        <Text style={s.sectionName}>{sec.title}</Text>
                        <Text style={s.sectionFinding}>{sec.finding}</Text>
                    </View>
                ))}

                {/* Checklist */}
                <Text style={s.sectionTitle}>Prosecution Readiness Checklist</Text>
                {CHECKLIST.map((item, i) => (
                    <View key={i} style={s.checkItem}>
                        <View style={[s.checkBox, item.done && s.checkBoxDone]}>
                            <Text style={[s.checkMark, item.done && s.checkMarkDone]}>{item.done ? '✓' : ''}</Text>
                        </View>
                        <Text style={[s.checkText, item.done && s.checkTextDone]}>{item.item}</Text>
                    </View>
                ))}

                <TouchableOpacity style={s.generateBtn} onPress={() => Alert.alert('📄 Report Generated', 'Challan scrutiny report generated and ready for review by the Supervisory Officer.')}>
                    <Text style={s.generateBtnText}>Generate Scrutiny Report</Text>
                </TouchableOpacity>
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#92400E', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#FDE68A', marginTop: 4 },
    scoreRow: { flexDirection: 'row', alignItems: 'center', marginTop: 14, gap: 14 },
    scoreCircle: { width: 56, height: 56, borderRadius: 28, backgroundColor: 'rgba(255,255,255,0.2)', justifyContent: 'center', alignItems: 'center', borderWidth: 3, borderColor: '#FDE68A' },
    scoreText: { fontSize: 18, fontWeight: '800', color: '#fff' },
    scoreLabel: { fontSize: 14, fontWeight: '700', color: '#fff' },
    scoreMeta: { fontSize: 12, color: '#FDE68A' },
    body: { flex: 1, padding: 16 },
    sectionTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 12, marginTop: 8 },
    sectionCard: { backgroundColor: '#fff', borderRadius: 12, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0', borderLeftWidth: 4 },
    sectionTop: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 4 },
    codeBadge: { backgroundColor: '#FFFBEB', paddingHorizontal: 10, paddingVertical: 3, borderRadius: 6 },
    codeText: { fontSize: 12, fontWeight: '700', color: '#92400E' },
    sectionName: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 4 },
    sectionFinding: { fontSize: 13, color: '#64748B', lineHeight: 20 },
    checkItem: { flexDirection: 'row', alignItems: 'center', backgroundColor: '#fff', padding: 14, borderRadius: 10, marginBottom: 6, borderWidth: 1, borderColor: '#E2E8F0' },
    checkBox: { width: 24, height: 24, borderRadius: 6, borderWidth: 2, borderColor: '#CBD5E1', justifyContent: 'center', alignItems: 'center', marginRight: 12 },
    checkBoxDone: { backgroundColor: '#DCFCE7', borderColor: '#16A34A' },
    checkMark: { fontSize: 14, fontWeight: '800', color: '#CBD5E1' },
    checkMarkDone: { color: '#16A34A' },
    checkText: { fontSize: 14, color: '#334155', flex: 1 },
    checkTextDone: { color: '#94A3B8', textDecorationLine: 'line-through' },
    generateBtn: { backgroundColor: '#92400E', padding: 16, borderRadius: 12, alignItems: 'center', marginTop: 16 },
    generateBtnText: { color: '#fff', fontWeight: '700', fontSize: 16 },
});
