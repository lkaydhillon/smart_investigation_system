// @ts-nocheck
import React from 'react';
import { View, Text, SafeAreaView, StyleSheet } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

export const MapScreen = () => {
    const insets = useSafeAreaInsets();

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <View style={s.header}>
                <Text style={s.headerTitle}>Crime Hotspot Map</Text>
                <Text style={s.headerSub}>Live case & complaint tracking</Text>
            </View>
            <View style={s.body}>
                <Text style={{ fontSize: 60, marginBottom: 20 }}>🗺️</Text>
                <Text style={s.title}>Map View Unavailable on Web</Text>
                <Text style={s.desc}>
                    The interactive crime hotspot map requires native GPS and rendering capabilities.
                    Please use the Smart Investigation mobile app (Android/iOS) to view live live officer tracking and crime hotspots.
                </Text>
            </View>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#fff', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    headerTitle: { fontSize: 24, fontWeight: '800', color: '#0F172A' },
    headerSub: { fontSize: 13, color: '#94A3B8', marginTop: 4 },
    body: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 32 },
    title: { fontSize: 18, fontWeight: '700', color: '#334155', marginBottom: 12 },
    desc: { fontSize: 14, color: '#64748B', textAlign: 'center', lineHeight: 22 },
});
