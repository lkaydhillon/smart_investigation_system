import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { Ionicons } from '@expo/vector-icons';
import { View, Text, TouchableOpacity, ScrollView, StyleSheet, Alert } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useAuthStore } from '../store/authStore';

import { DashboardScreen } from '../features/cases/screens/DashboardScreen';
import { CasesStackNavigator } from '../features/cases/navigation/CasesStackNavigator';
import { EvidenceCaptureScreen } from '../features/evidence/screens/EvidenceCaptureScreen';
import { MapScreen } from '../features/cases/screens/MapScreen';
import { ProfileScreen } from '../features/profile/screens/ProfileScreen';
import { ComplaintsScreen } from '../features/complaints/screens/ComplaintsScreen';
import { PersonsScreen } from '../features/persons/screens/PersonsScreen';
import { InvestigationScreen } from '../features/investigation/screens/InvestigationScreen';
import { LegalScreen } from '../features/legal/screens/LegalScreen';
import { DocumentsScreen } from '../features/documents/screens/DocumentsScreen';
import { AIAssistantScreen } from '../features/ai/screens/AIAssistantScreen';
import { SearchScreen } from '../features/search/screens/SearchScreen';
import { ComplianceScreen } from '../features/compliance/screens/ComplianceScreen';
import { SupervisoryDashboardScreen } from '../features/supervisory/screens/SupervisoryDashboardScreen';
import { VoiceRecordingScreen } from '../features/voice/screens/VoiceRecordingScreen';
import { CaseTimelineScreen } from '../features/timeline/screens/CaseTimelineScreen';
import { ChallanScrutinyScreen } from '../features/challan/screens/ChallanScrutinyScreen';

const Tab = createBottomTabNavigator();
const MoreStack = createNativeStackNavigator();

// @ts-ignore
const MoreMenuScreen = ({ navigation }: any) => {
    const insets = useSafeAreaInsets();
    const user = useAuthStore(s => s.user);
    const logout = useAuthStore(s => s.logout);

    const handleLogout = () => {
        Alert.alert('Logout', 'Are you sure you want to logout?', [
            { text: 'Cancel', style: 'cancel' },
            { text: 'Logout', style: 'destructive', onPress: logout }
        ]);
    };
    const sections = [
        {
            title: 'CORE MODULES',
            modules: [
                { name: 'Evidence Capture', icon: '📸', desc: 'Secure hashing & photos', screen: 'Evidence', bg: '#EFF6FF', border: '#BFDBFE' },
                { name: 'Persons Registry', icon: '👥', desc: 'Suspects, Victims, Witnesses', screen: 'Persons', bg: '#FAF5FF', border: '#E9D5FF' },
                { name: 'Investigation', icon: '🔍', desc: 'SOP steps, diary & interrogations', screen: 'Investigation', bg: '#EFF6FF', border: '#BFDBFE' },
                { name: 'Legal & Court', icon: '⚖️', desc: 'BNS sections, hearings, bail', screen: 'Legal', bg: '#FEF2F2', border: '#FECACA' },
                { name: 'Documents', icon: '📄', desc: 'FIR, Chargesheet, Memo generation', screen: 'Documents', bg: '#F0FDF4', border: '#BBF7D0' },
            ]
        },
        {
            title: 'AI & ANALYTICS',
            modules: [
                { name: 'AI Assistant', icon: '🤖', desc: 'Legal AI, document drafting, Q&A', screen: 'AIAssistant', bg: '#EEF2FF', border: '#C7D2FE' },
                { name: 'Advanced Search', icon: '🔎', desc: 'Fuzzy search & NL queries', screen: 'Search', bg: '#F0FDFA', border: '#99F6E4' },
                { name: 'Case Timeline', icon: '📊', desc: 'Event timeline & visualization', screen: 'CaseTimeline', bg: '#ECFEFF', border: '#A5F3FC' },
                { name: 'Challan Scrutiny', icon: '📋', desc: 'AI chargesheet review', screen: 'ChallanScrutiny', bg: '#FFFBEB', border: '#FDE68A' },
            ]
        },
        {
            title: 'SUPERVISION',
            modules: [
                { name: 'Supervisory Dashboard', icon: '📈', desc: 'Pendency & performance reports', screen: 'Supervisory', bg: '#ECFDF5', border: '#A7F3D0' },
                { name: 'Compliance Monitor', icon: '⏰', desc: 'Bail, POCSO & deadline alerts', screen: 'Compliance', bg: '#FFF1F2', border: '#FECDD3' },
            ]
        },
        {
            title: 'TOOLS',
            modules: [
                { name: 'Voice Recording', icon: '🎤', desc: 'Speech-to-Text capture', screen: 'VoiceRecording', bg: '#F8FAFC', border: '#CBD5E1' },
                { name: 'Profile & Settings', icon: '👤', desc: 'Officer profile', screen: 'Profile', bg: '#F8FAFC', border: '#E2E8F0' },
            ]
        },
    ];

    return (
        <View style={[ms.container, { paddingTop: insets.top }]}>
            <ScrollView style={ms.body} showsVerticalScrollIndicator={false}>
                <View style={ms.topBar}>
                    <View>
                        <Text style={ms.title}>All Modules</Text>
                        <Text style={ms.subtitle}>Smart AI Case Management System</Text>
                    </View>
                    <TouchableOpacity onPress={handleLogout} style={ms.logoutBtn}>
                        <Text style={ms.logoutBtnText}>⏻ Logout</Text>
                    </TouchableOpacity>
                </View>
                <Text style={ms.greeting}>👋 Welcome, {user?.fullName || user?.username || 'Officer'}</Text>

                {sections.map(section => (
                    <View key={section.title}>
                        <Text style={ms.sectionTitle}>{section.title}</Text>
                        {section.modules.map(mod => (
                            <TouchableOpacity key={mod.screen} onPress={() => navigation.navigate(mod.screen)}
                                style={[ms.moduleCard, { backgroundColor: mod.bg, borderColor: mod.border }]}>
                                <Text style={{ fontSize: 24, marginRight: 14 }}>{mod.icon}</Text>
                                <View style={{ flex: 1 }}>
                                    <Text style={ms.moduleName}>{mod.name}</Text>
                                    <Text style={ms.moduleDesc}>{mod.desc}</Text>
                                </View>
                                <Text style={{ fontSize: 20, color: '#94A3B8' }}>›</Text>
                            </TouchableOpacity>
                        ))}
                    </View>
                ))}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const ms = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    body: { flex: 1, paddingHorizontal: 16, paddingTop: 16 },
    topBar: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 4 },
    title: { fontSize: 24, fontWeight: '800', color: '#0F172A' },
    subtitle: { fontSize: 13, color: '#94A3B8' },
    greeting: { fontSize: 14, color: '#475569', marginBottom: 12, fontWeight: '500' },
    logoutBtn: { backgroundColor: '#FEE2E2', paddingHorizontal: 14, paddingVertical: 8, borderRadius: 10, borderWidth: 1, borderColor: '#FECACA' },
    logoutBtnText: { color: '#DC2626', fontWeight: '700', fontSize: 13 },
    sectionTitle: { fontSize: 11, fontWeight: '700', color: '#94A3B8', letterSpacing: 1, marginTop: 20, marginBottom: 8 },
    moduleCard: { flexDirection: 'row', alignItems: 'center', padding: 16, borderRadius: 14, marginBottom: 8, borderWidth: 1.5 },
    moduleName: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    moduleDesc: { fontSize: 12, color: '#64748B', marginTop: 2 },
});

const MoreNavigator = () => (
    <MoreStack.Navigator screenOptions={{ headerShown: false }}>
        <MoreStack.Screen name="MoreMenu" component={MoreMenuScreen} />
        <MoreStack.Screen name="Complaints" component={ComplaintsScreen} />
        <MoreStack.Screen name="Persons" component={PersonsScreen} />
        <MoreStack.Screen name="Investigation" component={InvestigationScreen} />
        <MoreStack.Screen name="Legal" component={LegalScreen} />
        <MoreStack.Screen name="Documents" component={DocumentsScreen} />
        <MoreStack.Screen name="AIAssistant" component={AIAssistantScreen} />
        <MoreStack.Screen name="Search" component={SearchScreen} />
        <MoreStack.Screen name="Compliance" component={ComplianceScreen} />
        <MoreStack.Screen name="Supervisory" component={SupervisoryDashboardScreen} />
        <MoreStack.Screen name="VoiceRecording" component={VoiceRecordingScreen} />
        <MoreStack.Screen name="CaseTimeline" component={CaseTimelineScreen} />
        <MoreStack.Screen name="ChallanScrutiny" component={ChallanScrutinyScreen} />
        <MoreStack.Screen name="Profile" component={ProfileScreen} />
    </MoreStack.Navigator>
);

export const MainTabNavigator = () => {
    return (
        <Tab.Navigator
            screenOptions={({ route }) => ({
                headerShown: false,
                tabBarActiveTintColor: '#1D4ED8',
                tabBarInactiveTintColor: '#94A3B8',
                tabBarStyle: { backgroundColor: '#fff', borderTopColor: '#E2E8F0', paddingBottom: 4, height: 56 },
                tabBarLabelStyle: { fontSize: 11, fontWeight: '600' },
                tabBarIcon: ({ color }) => {
                    let iconName: any;
                    if (route.name === 'Dashboard') iconName = 'home';
                    else if (route.name === 'Cases') iconName = 'folder';
                    else if (route.name === 'Complaints') iconName = 'document-text';
                    else if (route.name === 'Map') iconName = 'map';
                    else if (route.name === 'More') iconName = 'grid';
                    return <Ionicons name={iconName} size={22} color={color} />;
                },
            })}
        >
            <Tab.Screen name="Dashboard" component={DashboardScreen} options={{ tabBarLabel: 'Home' }} />
            <Tab.Screen name="Cases" component={CasesStackNavigator} options={{ tabBarLabel: 'Registry' }} />
            <Tab.Screen name="Complaints" component={ComplaintsScreen} options={{ tabBarLabel: 'Complaints' }} />
            <Tab.Screen name="Map" component={MapScreen} options={{ tabBarLabel: 'Map' }} />
            <Tab.Screen name="More" component={MoreNavigator} options={{ tabBarLabel: 'More' }} />
        </Tab.Navigator>
    );
};
