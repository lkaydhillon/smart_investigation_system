// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, Alert, StyleSheet, StatusBar, Clipboard } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useMutation } from '@tanstack/react-query';
import { apiClient } from '../../../core/api';
import { Speech } from 'expo-speech';

const DOC_TYPES = [
    { key: 'FIR', label: '📜 FIR', desc: 'First Information Report' },
    { key: 'Chargesheet', label: '📋 Chargesheet', desc: 'Final investigation report' },
    { key: 'Arrest Memo', label: '🔒 Arrest Memo', desc: 'Formal arrest memorandum' },
    { key: 'Search Warrant', label: '🔍 Search Warrant', desc: 'Authorization to search premises' },
    { key: 'Notice 41A', label: '📩 Notice 41A', desc: 'CrPC notice in lieu of arrest' },
    { key: 'Bail Application', label: '⚖️ Bail Application', desc: 'Application for bail' },
    { key: 'Remand Application', label: '🏛️ Remand Application', desc: 'Application for judicial remand' },
];

export const DocumentsScreen = () => {
    const insets = useSafeAreaInsets();
    const [language, setLanguage] = useState<'en' | 'hi'>('en');
    const [selectedDocType, setSelectedDocType] = useState('FIR');
    const [caseDesc, setCaseDesc] = useState('');
    const [accusedName, setAccusedName] = useState('');
    const [incidentDate, setIncidentDate] = useState('');
    const [location, setLocation] = useState('');
    const [generatedDoc, setGeneratedDoc] = useState<string | null>(null);
    const [isSpeaking, setIsSpeaking] = useState(false);

    const generateMutation = useMutation({
        mutationFn: async () => {
            const contextData = {
                documentType: selectedDocType,
                language,
                caseDescription: caseDesc,
                accusedName: accusedName || 'Unknown',
                incidentDate: incidentDate || new Date().toLocaleDateString('en-IN'),
                location: location || 'As reported',
                generatedAt: new Date().toISOString(),
            };
            const res = await apiClient.post('/AI/draft-document', {
                type: selectedDocType,
                documentType: selectedDocType,
                caseDescription: caseDesc,
                language,
                contextData,
            });
            return res.data?.DraftContent || res.data?.document || res.data;
        },
        onSuccess: (data: string) => {
            setGeneratedDoc(typeof data === 'string' ? data : JSON.stringify(data, null, 2));
        },
        onError: () => {
            // Offline fallback: generate a local draft
            const fallbackEn = `--- ${selectedDocType.toUpperCase()} DRAFT ---\n\nTo: The Station House Officer\nDate: ${incidentDate || new Date().toLocaleDateString('en-IN')}\nLocation: ${location || 'N/A'}\n\nSubject: ${selectedDocType} for incident reported\n\nFacts of the Case:\n${caseDesc || 'Case facts not provided'}\n\nAccused: ${accusedName || 'Unknown'}\n\nApplicable Sections: [To be determined by Investigating Officer]\n\nPrayer: It is respectfully prayed that appropriate legal action be taken.\n\n--- END OF DRAFT ---\n\n(Generated offline - Sync with server when connected)`;
            const fallbackHi = `--- ${selectedDocType.toUpperCase()} मसौदा ---\n\nसेवा में,\nथाना प्रभारी महोदय\nदिनांक: ${incidentDate || new Date().toLocaleDateString('hi-IN')}\nस्थान: ${location || 'N/A'}\n\nविषय: ${selectedDocType} - रिपोर्ट की गई घटना के लिए\n\nमामले के तथ्य:\n${caseDesc || 'मामले के तथ्य नहीं दिए गए'}\n\nआरोपी: ${accusedName || 'अज्ञात'}\n\nलागू धाराएं: [अन्वेषण अधिकारी द्वारा निर्धारित की जाएंगी]\n\nप्रार्थना: अनुरोध है कि उचित कानूनी कार्रवाई की जाए।\n\n--- मसौदे का अंत ---\n\n(ऑफ़लाइन जनरेट किया गया - कनेक्ट होने पर सर्वर से सिंक करें)`;
            setGeneratedDoc(language === 'hi' ? fallbackHi : fallbackEn);
        },
    });

    const handleSpeak = async () => {
        if (!generatedDoc) return;
        if (isSpeaking) {
            Speech.stop();
            setIsSpeaking(false);
            return;
        }
        setIsSpeaking(true);
        Speech.speak(generatedDoc, {
            language: language === 'hi' ? 'hi-IN' : 'en-IN',
            rate: 0.9,
            onDone: () => setIsSpeaking(false),
            onError: () => setIsSpeaking(false),
        });
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#065F46" />
            <View style={s.header}>
                <View style={s.headerTopRow}>
                    <View style={{ flex: 1 }}>
                        <Text style={s.headerTitle}>📄 AI Document Generator</Text>
                        <Text style={s.headerSub}>AI generates real legal documents from your case facts</Text>
                    </View>
                    <View style={s.langToggle}>
                        <TouchableOpacity onPress={() => setLanguage('en')} style={[s.langBtn, language === 'en' && s.langBtnActive]}>
                            <Text style={[s.langText, language === 'en' && s.langTextActive]}>EN</Text>
                        </TouchableOpacity>
                        <TouchableOpacity onPress={() => setLanguage('hi')} style={[s.langBtn, language === 'hi' && s.langBtnActive]}>
                            <Text style={[s.langText, language === 'hi' && s.langTextActive]}>हिं</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>

                {/* Document Type Selector */}
                <Text style={s.sectionLabel}>📁 Select Document Type</Text>
                <ScrollView horizontal showsHorizontalScrollIndicator={false} style={{ marginBottom: 16 }}>
                    {DOC_TYPES.map(dt => (
                        <TouchableOpacity key={dt.key} onPress={() => setSelectedDocType(dt.key)}
                            style={[s.typeChip, selectedDocType === dt.key && s.typeChipActive]}>
                            <Text style={[s.typeChipText, selectedDocType === dt.key && s.typeChipTextActive]}>{dt.label}</Text>
                        </TouchableOpacity>
                    ))}
                </ScrollView>

                {/* Case Details Form */}
                <View style={s.card}>
                    <Text style={s.cardTitle}>📝 Case Details (AI will use these to generate the document)</Text>

                    <Text style={s.fieldLabel}>Case / Incident Description *</Text>
                    <TextInput
                        style={s.textArea}
                        placeholder="Describe the incident in detail: what happened, when, how, witnesses, evidence found..."
                        placeholderTextColor="#94A3B8"
                        multiline
                        numberOfLines={5}
                        textAlignVertical="top"
                        value={caseDesc}
                        onChangeText={setCaseDesc}
                    />

                    <Text style={s.fieldLabel}>Accused Person Name</Text>
                    <TextInput style={s.input} placeholder="Name of accused (if known)" placeholderTextColor="#94A3B8"
                        value={accusedName} onChangeText={setAccusedName} />

                    <Text style={s.fieldLabel}>Incident Date</Text>
                    <TextInput style={s.input} placeholder="e.g. 16/03/2026" placeholderTextColor="#94A3B8"
                        value={incidentDate} onChangeText={setIncidentDate} />

                    <Text style={s.fieldLabel}>Location</Text>
                    <TextInput style={s.input} placeholder="Location of incident" placeholderTextColor="#94A3B8"
                        value={location} onChangeText={setLocation} />

                    <TouchableOpacity
                        onPress={() => generateMutation.mutate()}
                        disabled={!caseDesc.trim() || generateMutation.isPending}
                        style={[s.generateBtn, !caseDesc.trim() && s.btnDisabled]}>
                        {generateMutation.isPending
                            ? <><ActivityIndicator color="#fff" size="small" /><Text style={s.generateBtnText}> Gemini AI generating...</Text></>
                            : <Text style={s.generateBtnText}>🤖 Generate {selectedDocType} with AI</Text>}
                    </TouchableOpacity>
                </View>

                {/* Generated Document Output */}
                {generatedDoc && (
                    <View style={s.outputCard}>
                        <View style={s.outputHeader}>
                            <Text style={s.outputTitle}>✅ Generated {selectedDocType}</Text>
                            <View style={{ flexDirection: 'row', gap: 8 }}>
                                <TouchableOpacity style={s.actionBtn} onPress={handleSpeak}>
                                    <Text style={s.actionBtnText}>{isSpeaking ? '⏹ Stop' : '🔊 Read'}</Text>
                                </TouchableOpacity>
                                <TouchableOpacity style={s.actionBtn} onPress={() => {
                                    Clipboard.setString(generatedDoc);
                                    Alert.alert('Copied', 'Document copied to clipboard');
                                }}>
                                    <Text style={s.actionBtnText}>📋 Copy</Text>
                                </TouchableOpacity>
                            </View>
                        </View>
                        <Text style={s.outputText}>{generatedDoc}</Text>
                        <TouchableOpacity style={s.newBtn} onPress={() => setGeneratedDoc(null)}>
                            <Text style={s.newBtnText}>+ Generate New Document</Text>
                        </TouchableOpacity>
                    </View>
                )}

                <View style={{ height: 40 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#065F46', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTopRow: { flexDirection: 'row', alignItems: 'flex-start' },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 12, color: '#6EE7B7', marginTop: 4 },
    langToggle: { flexDirection: 'row', backgroundColor: 'rgba(255,255,255,0.15)', borderRadius: 20, padding: 4, marginLeft: 10 },
    langBtn: { paddingHorizontal: 12, paddingVertical: 6, borderRadius: 16 },
    langBtnActive: { backgroundColor: '#fff' },
    langText: { fontSize: 13, fontWeight: '700', color: '#A7F3D0' },
    langTextActive: { color: '#065F46' },
    body: { flex: 1, padding: 16 },
    sectionLabel: { fontSize: 13, fontWeight: '700', color: '#0F172A', marginBottom: 10 },
    typeChip: { paddingHorizontal: 14, paddingVertical: 10, borderRadius: 10, backgroundColor: '#F1F5F9', borderWidth: 1.5, borderColor: '#E2E8F0', marginRight: 8 },
    typeChipActive: { backgroundColor: '#ECFDF5', borderColor: '#059669' },
    typeChipText: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    typeChipTextActive: { color: '#065F46' },
    card: { backgroundColor: '#fff', borderRadius: 16, padding: 18, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0', elevation: 2, shadowColor: '#000', shadowOpacity: 0.05, shadowRadius: 8, shadowOffset: { width: 0, height: 2 } },
    cardTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 14 },
    fieldLabel: { fontSize: 12, fontWeight: '600', color: '#475569', marginBottom: 6, marginTop: 10 },
    textArea: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', minHeight: 110, borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 4 },
    input: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 12, fontSize: 14, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 4 },
    generateBtn: { backgroundColor: '#065F46', borderRadius: 12, padding: 16, alignItems: 'center', flexDirection: 'row', justifyContent: 'center', marginTop: 16 },
    generateBtnText: { color: '#fff', fontWeight: '700', fontSize: 15 },
    btnDisabled: { backgroundColor: '#CBD5E1' },
    outputCard: { backgroundColor: '#0F172A', borderRadius: 16, padding: 18, marginBottom: 16 },
    outputHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 },
    outputTitle: { fontSize: 14, fontWeight: '700', color: '#34D399' },
    actionBtn: { backgroundColor: 'rgba(255,255,255,0.12)', paddingHorizontal: 12, paddingVertical: 6, borderRadius: 8 },
    actionBtnText: { color: '#A7F3D0', fontWeight: '600', fontSize: 12 },
    outputText: { fontSize: 13, color: '#CBD5E1', lineHeight: 22, fontFamily: 'monospace' },
    newBtn: { backgroundColor: 'rgba(255,255,255,0.08)', padding: 12, borderRadius: 10, alignItems: 'center', marginTop: 16 },
    newBtnText: { color: '#6EE7B7', fontWeight: '600', fontSize: 13 },
});
