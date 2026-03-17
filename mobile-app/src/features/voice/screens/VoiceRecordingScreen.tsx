// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, StyleSheet, StatusBar, Alert } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Audio } from 'expo-av';
import { API_BASE_URL } from '../../../core/api';
import { useAuthStore } from '../../../store/authStore';

export const VoiceRecordingScreen = () => {
    const insets = useSafeAreaInsets();
    const [recording, setRecording] = useState<Audio.Recording | null>(null);
    const [recordings, setRecordings] = useState<any[]>([]);
    const [isRecording, setIsRecording] = useState(false);
    const [language, setLanguage] = useState<'en' | 'hi'>('en');

    const startRecording = async () => {
        try {
            await Audio.requestPermissionsAsync();
            await Audio.setAudioModeAsync({ allowsRecordingIOS: true, playsInSilentModeIOS: true });
            const { recording } = await Audio.Recording.createAsync(Audio.RecordingOptionsPresets.HIGH_QUALITY);
            setRecording(recording);
            setIsRecording(true);
        } catch (err: any) {
            Alert.alert('Microphone Error', 'Failed to start recording: ' + err.message);
        }
    };

    const stopRecording = async () => {
        if (!recording) return;
        try {
            setIsRecording(false);
            await recording.stopAndUnloadAsync();
            const uri = recording.getURI();
            setRecording(null);

            if (!uri) throw new Error("Audio file not generated");

            // Prepare multipart form data for the Backend API
            const token = useAuthStore.getState().token;
            const formData = new FormData();
            formData.append('audio', {
                uri,
                name: `recording_${Date.now()}.m4a`,
                type: 'audio/m4a'
            } as any);
            formData.append('language', language);

            // Send physical audio file to C# backend
            const response = await fetch(`${API_BASE_URL}/AI/speech-to-text`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData,
            });

            if (!response.ok) {
                const errText = await response.text();
                throw new Error(`Server Error (${response.status}): ${errText}`);
            }

            const result = await response.json();

            setRecordings(prev => [...prev, {
                id: Date.now().toString(),
                uri,
                duration: result.duration || '~recording',
                timestamp: new Date().toISOString(),
                sttResult: result.text || 'Transcription failed',
            }]);

            Alert.alert('✅ Recorded', 'Voice recording translated via Server AI successfully.');
        } catch (err: any) {
            Alert.alert('API Error', err.message || 'Failed to process audio on the server.');
        }
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#1E293B" />
            <View style={s.header}>
                <View style={s.headerTopRow}>
                    <View style={{ flex: 1 }}>
                        <Text style={s.headerTitle}>🎤 Voice Recording</Text>
                        <Text style={s.headerSub}>Speech-to-Text capture for complaints & statements</Text>
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
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* Record Button */}
                <View style={s.recordArea}>
                    {isRecording && <View style={s.pulseRing} pointerEvents="none" />}
                    <TouchableOpacity onPress={isRecording ? stopRecording : startRecording} style={[s.recordBtn, isRecording && s.recordBtnActive]}>
                        <Text style={{ fontSize: 40 }}>{isRecording ? '⏹' : '🎙️'}</Text>
                    </TouchableOpacity>
                    <Text style={s.recordLabel}>{isRecording ? 'Recording... Tap to Stop' : 'Tap to Start Recording'}</Text>
                </View>

                <Text style={s.sectionTitle}>Recordings ({recordings.length})</Text>
                {recordings.map((rec, i) => (
                    <View key={rec.id} style={s.recCard}>
                        <View style={s.recHeader}>
                            <Text style={s.recTitle}>Recording #{i + 1}</Text>
                            <Text style={s.recTime}>{new Date(rec.timestamp).toLocaleString('en-IN')}</Text>
                        </View>
                        <View style={s.sttBox}>
                            <Text style={s.sttLabel}>📝 Speech-to-Text Output:</Text>
                            <Text style={s.sttText}>{rec.sttResult}</Text>
                        </View>
                        <TouchableOpacity style={s.useBtn} onPress={() => Alert.alert('Copied', 'STT text copied to clipboard. You can paste it in complaint or diary forms.')}>
                            <Text style={s.useBtnText}>📋 Use in Complaint Form</Text>
                        </TouchableOpacity>
                    </View>
                ))}

                {recordings.length === 0 && (
                    <View style={s.emptyState}>
                        <Text style={{ fontSize: 48, marginBottom: 12 }}>🎤</Text>
                        <Text style={s.emptyTitle}>Voice-Based Capture</Text>
                        <Text style={s.emptyDesc}>Record verbal statements and complaints. The system will automatically convert speech to text for auto-filling complaint forms.</Text>
                    </View>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#1E293B', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerTopRow: { flexDirection: 'row', alignItems: 'flex-start' },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    headerSub: { fontSize: 13, color: '#94A3B8', marginTop: 4 },
    langToggle: { flexDirection: 'row', backgroundColor: 'rgba(255,255,255,0.1)', borderRadius: 20, padding: 4, marginLeft: 10 },
    langBtn: { paddingHorizontal: 12, paddingVertical: 6, borderRadius: 16 },
    langBtnActive: { backgroundColor: '#fff' },
    langText: { fontSize: 13, fontWeight: '700', color: '#CBD5E1' },
    langTextActive: { color: '#1E293B' },
    body: { flex: 1, padding: 16 },
    recordArea: { alignItems: 'center', marginVertical: 20 },
    recordBtn: { width: 100, height: 100, borderRadius: 50, backgroundColor: '#DC2626', justifyContent: 'center', alignItems: 'center', elevation: 4, shadowColor: '#DC2626', shadowOpacity: 0.3, shadowRadius: 16, shadowOffset: { width: 0, height: 4 } },
    recordBtnActive: { backgroundColor: '#1E293B', shadowColor: '#000' },
    recordLabel: { fontSize: 14, fontWeight: '600', color: '#475569', marginTop: 12 },
    pulseRing: { position: 'absolute', top: -6, width: 112, height: 112, borderRadius: 56, borderWidth: 3, borderColor: '#DC2626', opacity: 0.3 },
    sectionTitle: { fontSize: 14, fontWeight: '700', color: '#0F172A', marginBottom: 12 },
    recCard: { backgroundColor: '#fff', borderRadius: 16, padding: 16, marginBottom: 12, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1 },
    recHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 12 },
    recTitle: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    recTime: { fontSize: 12, color: '#94A3B8' },
    sttBox: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, marginBottom: 12, borderWidth: 1, borderColor: '#E2E8F0' },
    sttLabel: { fontSize: 12, fontWeight: '600', color: '#475569', marginBottom: 6 },
    sttText: { fontSize: 14, color: '#1E293B', lineHeight: 22 },
    useBtn: { backgroundColor: '#EFF6FF', padding: 12, borderRadius: 10, alignItems: 'center' },
    useBtnText: { color: '#1D4ED8', fontWeight: '700', fontSize: 13 },
    emptyState: { alignItems: 'center', marginTop: 30 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155' },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 24 },
});
