// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TextInput, TouchableOpacity, ActivityIndicator, StyleSheet, StatusBar } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery } from '@tanstack/react-query';
import { apiClient } from '../../../core/api';

const TIMELINE_EVENTS = [
    { date: '14 Mar 2026, 20:30', type: 'FIR Filed', desc: 'FIR registered under Section 103 BNS', icon: '📜', color: '#DC2626' },
    { date: '14 Mar 2026, 22:00', type: 'Evidence Collected', desc: 'Scene survey completed, 4 items seized', icon: '📸', color: '#7C3AED' },
    { date: '15 Mar 2026, 09:00', type: 'IO Assigned', desc: 'SI Anjali Mehta assigned as Investigation Officer', icon: '👮', color: '#2563EB' },
    { date: '15 Mar 2026, 14:00', type: 'Witness Statement', desc: 'Statement of eyewitness Ravi Kumar recorded', icon: '🗣️', color: '#0F766E' },
    { date: '16 Mar 2026, 10:00', type: 'Arrest', desc: 'Accused Vikas Gupta arrested from Sarojini Nagar', icon: '🔒', color: '#DC2626' },
    { date: '16 Mar 2026, 16:00', type: 'Remand', desc: 'Police remand of 4 days granted by Magistrate', icon: '⚖️', color: '#92400E' },
    { date: '17 Mar 2026, 11:00', type: 'Interrogation', desc: 'First interrogation session with accused conducted', icon: '❓', color: '#4338CA' },
    { date: '20 Mar 2026, 09:00', type: 'Forensic Report', desc: 'Fingerprint analysis report received from FSL', icon: '🔬', color: '#059669' },
];

export const CaseTimelineScreen = () => {
    const insets = useSafeAreaInsets();
    const [caseId, setCaseId] = useState('');

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#0E7490" />
            <View style={s.header}>
                <Text style={s.headerTitle}>📊 Case Timeline</Text>
                <Text style={s.headerSub}>Chronological event visualization</Text>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {TIMELINE_EVENTS.map((event, i) => (
                    <View key={i} style={s.timelineRow}>
                        {/* Timeline line */}
                        <View style={s.timelineCol}>
                            <View style={[s.dot, { backgroundColor: event.color }]} />
                            {i < TIMELINE_EVENTS.length - 1 && <View style={s.line} />}
                        </View>
                        {/* Event card */}
                        <View style={s.eventCard}>
                            <View style={s.eventHeader}>
                                <Text style={{ fontSize: 18, marginRight: 8 }}>{event.icon}</Text>
                                <View style={{ flex: 1 }}>
                                    <Text style={s.eventType}>{event.type}</Text>
                                    <Text style={s.eventDate}>{event.date}</Text>
                                </View>
                            </View>
                            <Text style={s.eventDesc}>{event.desc}</Text>
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
    header: { backgroundColor: '#0E7490', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#A5F3FC', marginTop: 4 },
    body: { flex: 1, padding: 16 },
    timelineRow: { flexDirection: 'row', marginBottom: 0 },
    timelineCol: { width: 32, alignItems: 'center' },
    dot: { width: 14, height: 14, borderRadius: 7, borderWidth: 3, borderColor: '#fff', elevation: 2, shadowColor: '#000', shadowOpacity: 0.1, shadowRadius: 4 },
    line: { width: 2, flex: 1, backgroundColor: '#E2E8F0', marginVertical: 2 },
    eventCard: { flex: 1, backgroundColor: '#fff', borderRadius: 12, padding: 14, marginLeft: 8, marginBottom: 12, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    eventHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 6 },
    eventType: { fontSize: 14, fontWeight: '700', color: '#0F172A' },
    eventDate: { fontSize: 11, color: '#94A3B8' },
    eventDesc: { fontSize: 13, color: '#475569', lineHeight: 20 },
});
