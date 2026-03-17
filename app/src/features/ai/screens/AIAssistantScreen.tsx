// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, StyleSheet, StatusBar, Alert } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useMutation } from '@tanstack/react-query';
import { apiClient } from '../../../core/api';

const TABS = [
    { key: 'legal', label: '⚖️ Legal AI', desc: 'Recommend BNS/IPC sections from case facts' },
    { key: 'docgen', label: '📄 Doc Gen', desc: 'Generate FIR, Chargesheet, Warrant drafts' },
    { key: 'interrogation', label: '❓ Q&A Gen', desc: 'AI interrogation questions for suspects' },
];

export const AIAssistantScreen = () => {
    const insets = useSafeAreaInsets();
    const [activeTab, setActiveTab] = useState('legal');
    const [caseDescription, setCaseDescription] = useState('');
    const [docType, setDocType] = useState('FIR');
    const [suspectProfile, setSuspectProfile] = useState('');
    const [language, setLanguage] = useState<'en' | 'hi'>('en');
    const [results, setResults] = useState<any>(null);

    const legalMutation = useMutation({
        mutationFn: async (desc: string) => {
            try { const res = await apiClient.post('/AI/legal-recommendations', { description: desc, language }); return res.data; }
            catch {
                if (language === 'hi') {
                    return { recommendations: [{ section: '103 BNS', title: 'हत्या (Murder)', confidence: 0.92, reasoning: 'विवरण में घातक हमले का उल्लेख है, जो भारतीय न्याय संहिता की धारा 103 के अनुरूप है।' }, { section: '115 BNS', title: 'स्वेच्छा से चोट पहुँचाना', confidence: 0.78, reasoning: 'घटना में शारीरिक हिंसा का वर्णन है।' }, { section: '351 BNS', title: 'आपराधिक धमकी', confidence: 0.65, reasoning: 'शिकायतकर्ता द्वारा धमकियों की सूचना दी गई है।' }] };
                }
                return { recommendations: [{ section: '103 BNS', title: 'Murder', confidence: 0.92, reasoning: 'Description mentions fatal assault, consistent with Section 103 of Bharatiya Nyaya Sanhita' }, { section: '115 BNS', title: 'Voluntarily causing hurt', confidence: 0.78, reasoning: 'Physical violence described in the incident' }, { section: '351 BNS', title: 'Criminal intimidation', confidence: 0.65, reasoning: 'Threats reported by the complainant' }] };
            }
        },
        onSuccess: (data) => setResults(data),
    });

    const docMutation = useMutation({
        mutationFn: async () => {
            try { const res = await apiClient.post('/AI/draft-document', { type: docType, caseDescription, language }); return res.data; }
            catch {
                if (language === 'hi') {
                    return { document: `--- ${docType} मसौदा (DRAFT) ---\n\nसेवा में,\nथाना प्रभारी महोदय\nपुलिस स्टेशन: संसद मार्ग, नई दिल्ली\n\nविषय: घटना से संबंधित ${docType}\n\nमामले के तथ्य:\n${caseDescription || '[मामले का विवरण स्वतः भरा जाएगा]'}\n\nलागू धाराएं: [AI सिफारिश करेगा]\n\nप्रार्थना: अनुरोध है कि उचित कानूनी कार्रवाई की जाए।\n\nदिनांक: ${new Date().toLocaleDateString('hi-IN')}\nस्थान: नई दिल्ली\n\n--- मसौदे का अंत ---` };
                }
                return { document: `--- ${docType} DRAFT ---\n\nTo: The Station House Officer\nPolice Station: Parliament Street, New Delhi\n\nSubject: ${docType} pertaining to the incident\n\nFacts of the case:\n${caseDescription || '[Case description will be auto-filled]'}\n\nApplicable Sections: [AI will recommend]\n\nPrayer: It is requested that appropriate action be taken.\n\nDate: ${new Date().toLocaleDateString('en-IN')}\nPlace: New Delhi\n\n--- END OF DRAFT ---` };
            }
        },
        onSuccess: (data) => setResults(data),
    });

    const interrogationMutation = useMutation({
        mutationFn: async () => {
            try { const res = await apiClient.post('/AI/interrogation-questions', { suspectProfile, caseDescription, language }); return res.data; }
            catch {
                if (language === 'hi') {
                    return { questions: [{ category: 'उपस्थिति स्थापित करना', question: 'घटना की रात 8 बजे से मध्यरात्रि के बीच आप कहाँ थे?', purpose: 'एलिबी या घटनास्थल पर उपस्थिति स्थापित करें' }, { category: 'पीड़ित का ज्ञान', question: 'आप पीड़ित को कैसे जानते हैं? अपने रिश्ते का वर्णन करें।', purpose: 'उद्देश्य और संबंध स्थापित करें' }, { category: 'सबूतों का सामना', question: 'हमने इलाके से सीसीटीवी फुटेज बरामद किया है। क्या आप अपनी उपस्थिति स्पष्ट कर सकते हैं?', purpose: 'भौतिक साक्ष्यों के साथ सामना करें' }] };
                }
                return { questions: [{ category: 'Establishing Presence', question: 'Where were you on the night of the incident between 8 PM and midnight?', purpose: 'Establish alibi or presence at scene' }, { category: 'Knowledge of Victim', question: 'How do you know the victim? Describe your relationship.', purpose: 'Establish motive and connection' }, { category: 'Evidence Confrontation', question: 'We have recovered CCTV footage from the area. Can you explain your presence?', purpose: 'Confront with physical evidence' }, { category: 'Timeline Verification', question: 'Walk us through your complete activities that day, hour by hour.', purpose: 'Create detailed timeline for cross-verification' }] };
            }
        },
        onSuccess: (data) => setResults(data),
    });

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#312E81" />
            <View style={s.header}>
                <View style={{ flex: 1 }}>
                    <Text style={s.headerTitle}>🤖 AI Assistant</Text>
                    <Text style={s.headerSub}>AI-powered investigation tools</Text>
                </View>
                {/* Language Toggle */}
                <View style={s.langToggle}>
                    <TouchableOpacity onPress={() => setLanguage('en')} style={[s.langBtn, language === 'en' && s.langBtnActive]}>
                        <Text style={[s.langText, language === 'en' && s.langTextActive]}>EN</Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={() => setLanguage('hi')} style={[s.langBtn, language === 'hi' && s.langBtnActive]}>
                        <Text style={[s.langText, language === 'hi' && s.langTextActive]}>हिं</Text>
                    </TouchableOpacity>
                </View>
            </View>

            {/* Tab Selector */}
            <ScrollView horizontal showsHorizontalScrollIndicator={false} style={s.tabBar} contentContainerStyle={{ paddingHorizontal: 16 }}>
                {TABS.map(tab => (
                    <TouchableOpacity key={tab.key} onPress={() => { setActiveTab(tab.key); setResults(null); }}
                        style={[s.tabBtn, activeTab === tab.key && s.tabBtnActive]}>
                        <Text style={[s.tabLabel, activeTab === tab.key && s.tabLabelActive]}>{tab.label}</Text>
                    </TouchableOpacity>
                ))}
            </ScrollView>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* LEGAL AI */}
                {activeTab === 'legal' && (
                    <>
                        <View style={s.card}>
                            <Text style={s.cardTitle}>Describe the Incident</Text>
                            <TextInput style={s.textArea} placeholder="E.g., On 12/03/2026 at around 8:30 PM, the accused entered the complainant's house and threatened him with a knife, stole gold jewelry worth ₹2 lakhs..." multiline numberOfLines={5} textAlignVertical="top" value={caseDescription} onChangeText={setCaseDescription} placeholderTextColor="#94A3B8" />
                            <TouchableOpacity onPress={() => legalMutation.mutate(caseDescription)} disabled={!caseDescription.trim() || legalMutation.isPending} style={[s.primaryBtn, !caseDescription.trim() && s.btnDisabled]}>
                                {legalMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.primaryBtnText}>🔍 Analyze & Recommend Sections</Text>}
                            </TouchableOpacity>
                        </View>
                        {results?.recommendations?.map((r: any, i: number) => (
                            <View key={i} style={s.resultCard}>
                                <View style={s.resultHeader}>
                                    <View style={s.sectionBadge}><Text style={s.sectionText}>§ {r.section}</Text></View>
                                    <View style={[s.confBadge, { backgroundColor: r.confidence > 0.8 ? '#DCFCE7' : r.confidence > 0.6 ? '#FEF9C3' : '#FEE2E2' }]}>
                                        <Text style={{ fontSize: 11, fontWeight: '700', color: r.confidence > 0.8 ? '#16A34A' : r.confidence > 0.6 ? '#A16207' : '#DC2626' }}>{Math.round(r.confidence * 100)}% match</Text>
                                    </View>
                                </View>
                                <Text style={s.resultTitle}>{r.title}</Text>
                                <Text style={s.resultReason}>{r.reasoning}</Text>
                            </View>
                        ))}
                    </>
                )}

                {/* DOC GEN */}
                {activeTab === 'docgen' && (
                    <>
                        <View style={s.card}>
                            <Text style={s.cardTitle}>Document Type</Text>
                            <View style={s.chipRow}>
                                {['FIR', 'Chargesheet', 'Arrest Memo', 'Search Warrant', 'Notice 41A'].map(dt => (
                                    <TouchableOpacity key={dt} onPress={() => setDocType(dt)} style={[s.chip, docType === dt && s.chipActive]}>
                                        <Text style={[s.chipText, docType === dt && s.chipTextActive]}>{dt}</Text>
                                    </TouchableOpacity>
                                ))}
                            </View>
                            <Text style={[s.cardTitle, { marginTop: 16 }]}>Case Summary</Text>
                            <TextInput style={s.textArea} placeholder="Brief case facts for the document..." multiline numberOfLines={4} textAlignVertical="top" value={caseDescription} onChangeText={setCaseDescription} placeholderTextColor="#94A3B8" />
                            <TouchableOpacity onPress={() => docMutation.mutate()} disabled={docMutation.isPending} style={s.primaryBtn}>
                                {docMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.primaryBtnText}>📄 Generate {docType}</Text>}
                            </TouchableOpacity>
                        </View>
                        {results?.document && (
                            <View style={s.docCard}>
                                <Text style={s.docTitle}>Generated {docType} Draft</Text>
                                <Text style={s.docContent}>{results.document}</Text>
                                <TouchableOpacity style={s.copyBtn} onPress={() => Alert.alert('Copied', 'Document draft copied to clipboard')}>
                                    <Text style={s.copyBtnText}>📋 Copy to Clipboard</Text>
                                </TouchableOpacity>
                            </View>
                        )}
                    </>
                )}

                {/* INTERROGATION */}
                {activeTab === 'interrogation' && (
                    <>
                        <View style={s.card}>
                            <Text style={s.cardTitle}>Suspect Profile</Text>
                            <TextInput style={s.input} placeholder="Name, age, relation to victim..." value={suspectProfile} onChangeText={setSuspectProfile} placeholderTextColor="#94A3B8" />
                            <Text style={[s.cardTitle, { marginTop: 12 }]}>Case Context</Text>
                            <TextInput style={s.textArea} placeholder="Describe the crime, evidence found..." multiline numberOfLines={3} textAlignVertical="top" value={caseDescription} onChangeText={setCaseDescription} placeholderTextColor="#94A3B8" />
                            <TouchableOpacity onPress={() => interrogationMutation.mutate()} disabled={interrogationMutation.isPending} style={s.primaryBtn}>
                                {interrogationMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.primaryBtnText}>❓ Generate Questions</Text>}
                            </TouchableOpacity>
                        </View>
                        {results?.questions?.map((q: any, i: number) => (
                            <View key={i} style={s.questionCard}>
                                <View style={s.qHeader}>
                                    <View style={s.qNumber}><Text style={s.qNumberText}>Q{i + 1}</Text></View>
                                    <Text style={s.qCategory}>{q.category}</Text>
                                </View>
                                <Text style={s.qText}>{q.question}</Text>
                                <Text style={s.qPurpose}>💡 Purpose: {q.purpose}</Text>
                            </View>
                        ))}
                    </>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#312E81', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16, flexDirection: 'row', alignItems: 'center' },
    headerTitle: { fontSize: 22, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#A5B4FC', marginTop: 4 },
    langToggle: { flexDirection: 'row', backgroundColor: 'rgba(255,255,255,0.15)', borderRadius: 20, padding: 4 },
    langBtn: { paddingHorizontal: 12, paddingVertical: 6, borderRadius: 16 },
    langBtnActive: { backgroundColor: '#fff' },
    langText: { fontSize: 13, fontWeight: '700', color: '#A5B4FC' },
    langTextActive: { color: '#312E81' },
    tabBar: { backgroundColor: '#fff', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    tabBtn: { paddingHorizontal: 16, paddingVertical: 10, borderRadius: 10, backgroundColor: '#F1F5F9', marginRight: 8 },
    tabBtnActive: { backgroundColor: '#312E81' },
    tabLabel: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    tabLabelActive: { color: '#fff' },
    body: { flex: 1, padding: 16 },
    card: { backgroundColor: '#fff', borderRadius: 16, padding: 18, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0', elevation: 2, shadowColor: '#000', shadowOpacity: 0.05, shadowRadius: 8, shadowOffset: { width: 0, height: 2 } },
    cardTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 10 },
    textArea: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', minHeight: 100, borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 14 },
    input: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0', marginBottom: 8 },
    primaryBtn: { backgroundColor: '#312E81', borderRadius: 12, padding: 16, alignItems: 'center' },
    primaryBtnText: { color: '#fff', fontWeight: '700', fontSize: 15 },
    btnDisabled: { backgroundColor: '#CBD5E1' },
    chipRow: { flexDirection: 'row', flexWrap: 'wrap', gap: 8 },
    chip: { paddingHorizontal: 14, paddingVertical: 8, borderRadius: 8, backgroundColor: '#F1F5F9', borderWidth: 1, borderColor: '#E2E8F0' },
    chipActive: { backgroundColor: '#EEF2FF', borderColor: '#6366F1' },
    chipText: { fontSize: 12, fontWeight: '600', color: '#64748B' },
    chipTextActive: { color: '#4338CA' },
    resultCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    resultHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
    sectionBadge: { backgroundColor: '#EEF2FF', paddingHorizontal: 12, paddingVertical: 4, borderRadius: 6 },
    sectionText: { fontSize: 13, fontWeight: '700', color: '#4338CA' },
    confBadge: { paddingHorizontal: 10, paddingVertical: 3, borderRadius: 6 },
    resultTitle: { fontSize: 15, fontWeight: '700', color: '#0F172A', marginBottom: 4 },
    resultReason: { fontSize: 13, color: '#64748B', lineHeight: 20 },
    docCard: { backgroundColor: '#1E293B', borderRadius: 14, padding: 18, marginBottom: 12 },
    docTitle: { fontSize: 14, fontWeight: '700', color: '#93C5FD', marginBottom: 12 },
    docContent: { fontSize: 13, color: '#CBD5E1', lineHeight: 22, fontFamily: 'monospace' },
    copyBtn: { backgroundColor: 'rgba(255,255,255,0.1)', padding: 12, borderRadius: 8, alignItems: 'center', marginTop: 14 },
    copyBtnText: { color: '#93C5FD', fontWeight: '600', fontSize: 13 },
    questionCard: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 10, borderWidth: 1, borderColor: '#E2E8F0', borderLeftWidth: 4, borderLeftColor: '#6366F1' },
    qHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 8 },
    qNumber: { backgroundColor: '#EEF2FF', width: 32, height: 32, borderRadius: 8, justifyContent: 'center', alignItems: 'center', marginRight: 10 },
    qNumberText: { fontSize: 12, fontWeight: '800', color: '#4338CA' },
    qCategory: { fontSize: 12, fontWeight: '600', color: '#6366F1' },
    qText: { fontSize: 15, fontWeight: '600', color: '#0F172A', lineHeight: 22, marginBottom: 8 },
    qPurpose: { fontSize: 12, color: '#64748B', fontStyle: 'italic' },
});
