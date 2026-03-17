// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, Alert, StyleSheet, StatusBar, Platform } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { complaintsApi } from '../api/complaintsApi';

export const ComplaintsScreen = () => {
    const insets = useSafeAreaInsets();
    const queryClient = useQueryClient();
    const [showForm, setShowForm] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [form, setForm] = useState({ type: 0, description: '', complainantName: '', complainantPhone: '', isUrgent: false });

    const resetForm = () => {
        setForm({ type: 0, description: '', complainantName: '', complainantPhone: '', isUrgent: false });
        setEditingId(null);
        setShowForm(false);
    };

    const { data, isLoading, refetch } = useQuery({ queryKey: ['complaints'], queryFn: () => complaintsApi.getAll() });
    const complaints = data?.items || data || [];

    const createMutation = useMutation({
        mutationFn: async (formData: any) => {
            const payload = {
                ...formData,
                policeStationId: '198bd038-6432-4c4c-b776-61f8f012185f',
                dateOfIncident: new Date().toISOString(),
                isOfflineEntry: false,
                gpsLatitude: null,
                gpsLongitude: null,
            };
            console.log('[Complaints] Sending payload:', JSON.stringify(payload));
            const res = await complaintsApi.create(payload);
            console.log('[Complaints] Response:', JSON.stringify(res));
            // Check if it was queued offline (fake 202)
            if (res?._optimistic) {
                Alert.alert('Offline Mode', 'Complaint saved offline. Will sync when connected.');
                return res;
            }
            return res;
        },
        onSuccess: (data) => {
            queryClient.invalidateQueries({ queryKey: ['complaints'] });
            resetForm();
            if (!data?._optimistic) {
                Alert.alert('✅ Success', `Complaint ${data?.complaintNumber || ''} registered successfully!`);
            }
        },
        onError: (error: any) => {
            console.error('[Complaints] Error:', error?.response?.data || error?.message);
            Alert.alert('❌ Registration Failed', `${error?.response?.data?.error || error?.message || 'Unknown error. Check network connection.'}`);
        },
    });

    const updateMutation = useMutation({
        mutationFn: (formData: any) => complaintsApi.update(editingId!, formData),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['complaints'] }); resetForm(); Alert.alert('✅ Success', 'Complaint updated'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to update complaint'),
    });

    const deleteMutation = useMutation({
        mutationFn: complaintsApi.delete,
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['complaints'] }); Alert.alert('🗑️ Deleted', 'Complaint removed'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to delete complaint'),
    });

    const handleSubmit = () => {
        if (!form.complainantName.trim()) { Alert.alert('Required', 'Please enter complainant name'); return; }
        if (!form.description.trim()) { Alert.alert('Required', 'Please describe the incident'); return; }
        if (editingId) updateMutation.mutate(form);
        else createMutation.mutate(form);
    };

    const handleEdit = (complaint: any) => {
        setForm({ type: complaint.type || 0, description: complaint.description || '', complainantName: complaint.complainantName || '', complainantPhone: complaint.complainantPhone || '', isUrgent: complaint.isUrgent || false });
        setEditingId(complaint.id);
        setShowForm(true);
    };

    const handleDelete = (id: string) => {
        Alert.alert('Confirm Delete', 'Are you sure you want to delete this complaint?', [
            { text: 'Cancel', style: 'cancel' },
            { text: 'Delete', style: 'destructive', onPress: () => deleteMutation.mutate(id) }
        ]);
    };

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="dark-content" backgroundColor="#fff" />
            {/* Header */}
            <View style={s.header}>
                <View style={s.headerRow}>
                    <Text style={s.headerTitle}>Complaints</Text>
                    <TouchableOpacity onPress={() => showForm ? resetForm() : setShowForm(true)} style={[s.btn, showForm && s.btnOutline]}>
                        <Text style={[s.btnText, showForm && s.btnOutlineText]}>{showForm ? '✕ Cancel' : '+ New Complaint'}</Text>
                    </TouchableOpacity>
                </View>
                <Text style={s.headerSub}>Register & track public complaints</Text>
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {/* Form */}
                {showForm && (
                    <View style={s.formCard}>
                        <Text style={s.formTitle}>{editingId ? 'Edit Complaint' : 'Register New Complaint'}</Text>

                        <Text style={s.label}>Complainant Name *</Text>
                        <TextInput style={s.input} placeholder="Full name of complainant" value={form.complainantName} onChangeText={t => setForm(p => ({ ...p, complainantName: t }))} placeholderTextColor="#999" />

                        <Text style={s.label}>Phone Number</Text>
                        <TextInput style={s.input} placeholder="Mobile number" keyboardType="phone-pad" value={form.complainantPhone} onChangeText={t => setForm(p => ({ ...p, complainantPhone: t }))} placeholderTextColor="#999" />

                        <Text style={s.label}>Incident Description *</Text>
                        <TextInput style={[s.input, s.textArea]} placeholder="Describe the incident in detail..." multiline numberOfLines={5} textAlignVertical="top" value={form.description} onChangeText={t => setForm(p => ({ ...p, description: t }))} placeholderTextColor="#999" />

                        <Text style={s.label}>Complaint Type</Text>
                        <View style={s.row}>
                            {[{ v: 0, l: '📝 Written' }, { v: 1, l: '🗣 Verbal' }, { v: 2, l: '📩 Email' }].map(t => (
                                <TouchableOpacity key={t.v} onPress={() => setForm(p => ({ ...p, type: t.v }))} style={[s.chip, form.type === t.v && s.chipActive]}>
                                    <Text style={[s.chipText, form.type === t.v && s.chipTextActive]}>{t.l}</Text>
                                </TouchableOpacity>
                            ))}
                        </View>

                        <TouchableOpacity onPress={() => setForm(p => ({ ...p, isUrgent: !p.isUrgent }))} style={[s.urgentBtn, form.isUrgent && s.urgentBtnActive]}>
                            <Text style={[s.urgentText, form.isUrgent && s.urgentTextActive]}>⚠️ {form.isUrgent ? 'MARKED AS URGENT' : 'Mark as Urgent'}</Text>
                        </TouchableOpacity>

                        <TouchableOpacity onPress={handleSubmit} disabled={createMutation.isPending || updateMutation.isPending} style={[s.submitBtn, (createMutation.isPending || updateMutation.isPending) && { opacity: 0.6 }]}>
                            {createMutation.isPending || updateMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.submitText}>{editingId ? 'Update Complaint' : 'Submit Complaint'}</Text>}
                        </TouchableOpacity>
                    </View>
                )}

                {/* List */}
                {isLoading && <ActivityIndicator size="large" color="#1D4ED8" style={{ marginTop: 40 }} />}

                {complaints.map((item: any) => (
                    <View key={item.id} style={s.card}>
                        <View style={s.cardHeader}>
                            <Text style={s.cardNumber}>{item.complaintNumber || 'Draft'}</Text>
                            <View style={[s.badge, item.isUrgent ? s.badgeRed : s.badgeGreen]}>
                                <Text style={[s.badgeText, item.isUrgent ? s.badgeRedText : s.badgeGreenText]}>
                                    {item.isUrgent ? '🔴 URGENT' : item.status || 'Received'}
                                </Text>
                            </View>
                        </View>
                        <Text style={s.cardDesc} numberOfLines={2}>{item.description}</Text>
                        <View style={s.cardFooter}>
                            <Text style={s.cardMeta}>👤 {item.complainantName}</Text>
                            <Text style={s.cardMeta}>{new Date(item.createdDate || Date.now()).toLocaleDateString('en-IN')}</Text>
                        </View>
                        <View style={s.actionRow}>
                            <TouchableOpacity onPress={() => handleEdit(item)} style={s.iconBtn}><Text style={{ fontSize: 18 }}>✏️</Text></TouchableOpacity>
                            <TouchableOpacity onPress={() => handleDelete(item.id)} style={s.iconBtn}><Text style={{ fontSize: 18 }}>🗑️</Text></TouchableOpacity>
                        </View>
                    </View>
                ))}

                {!isLoading && complaints.length === 0 && !showForm && (
                    <View style={s.empty}>
                        <Text style={{ fontSize: 48, marginBottom: 12 }}>📋</Text>
                        <Text style={s.emptyTitle}>No Complaints Yet</Text>
                        <Text style={s.emptyDesc}>Tap "+ New Complaint" to register the first complaint.</Text>
                    </View>
                )}
                <View style={{ height: 40 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#fff', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16, borderBottomWidth: 1, borderBottomColor: '#E2E8F0' },
    headerRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
    headerTitle: { fontSize: 24, fontWeight: '800', color: '#0F172A' },
    headerSub: { fontSize: 13, color: '#94A3B8', marginTop: 4 },
    btn: { backgroundColor: '#1D4ED8', paddingHorizontal: 16, paddingVertical: 10, borderRadius: 10 },
    btnText: { color: '#fff', fontWeight: '700', fontSize: 14 },
    btnOutline: { backgroundColor: '#fff', borderWidth: 1.5, borderColor: '#DC2626' },
    btnOutlineText: { color: '#DC2626' },
    body: { flex: 1, paddingHorizontal: 16, paddingTop: 16 },
    formCard: { backgroundColor: '#fff', borderRadius: 16, padding: 20, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0', elevation: 2, shadowColor: '#000', shadowOpacity: 0.05, shadowRadius: 8, shadowOffset: { width: 0, height: 2 } },
    formTitle: { fontSize: 18, fontWeight: '700', color: '#0F172A', marginBottom: 16 },
    label: { fontSize: 13, fontWeight: '600', color: '#475569', marginBottom: 6, marginTop: 8 },
    input: { backgroundColor: '#F1F5F9', borderRadius: 10, padding: 14, fontSize: 15, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0' },
    textArea: { minHeight: 120, textAlignVertical: 'top' },
    row: { flexDirection: 'row', gap: 8, marginTop: 4 },
    chip: { flex: 1, backgroundColor: '#F1F5F9', paddingVertical: 10, borderRadius: 10, alignItems: 'center', borderWidth: 1.5, borderColor: '#E2E8F0' },
    chipActive: { backgroundColor: '#EFF6FF', borderColor: '#3B82F6' },
    chipText: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    chipTextActive: { color: '#1D4ED8' },
    urgentBtn: { backgroundColor: '#F1F5F9', padding: 14, borderRadius: 10, alignItems: 'center', marginTop: 16, borderWidth: 1.5, borderColor: '#E2E8F0' },
    urgentBtnActive: { backgroundColor: '#FEF2F2', borderColor: '#EF4444' },
    urgentText: { fontSize: 14, fontWeight: '600', color: '#64748B' },
    urgentTextActive: { color: '#DC2626', fontWeight: '700' },
    submitBtn: { backgroundColor: '#1D4ED8', padding: 16, borderRadius: 12, alignItems: 'center', marginTop: 16 },
    submitText: { color: '#fff', fontWeight: '700', fontSize: 16 },
    card: { backgroundColor: '#fff', borderRadius: 14, padding: 16, marginBottom: 12, borderWidth: 1, borderColor: '#E2E8F0', elevation: 1, shadowColor: '#000', shadowOpacity: 0.03, shadowRadius: 4, shadowOffset: { width: 0, height: 1 } },
    cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
    cardNumber: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    badge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 8 },
    badgeText: { fontSize: 11, fontWeight: '700' },
    badgeRed: { backgroundColor: '#FEF2F2' }, badgeRedText: { color: '#DC2626' },
    badgeGreen: { backgroundColor: '#F0FDF4' }, badgeGreenText: { color: '#16A34A' },
    cardDesc: { fontSize: 14, color: '#475569', lineHeight: 20 },
    cardFooter: { flexDirection: 'row', justifyContent: 'space-between', marginTop: 10 },
    cardMeta: { fontSize: 12, color: '#94A3B8' },
    actionRow: { flexDirection: 'row', gap: 8, marginTop: 12, borderTopWidth: 1, borderTopColor: '#F1F5F9', paddingTop: 10, justifyContent: 'flex-end' },
    iconBtn: { padding: 6, backgroundColor: '#F8FAFC', borderRadius: 8, borderWidth: 1, borderColor: '#E2E8F0' },
    empty: { alignItems: 'center', marginTop: 60 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155' },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 40 },
});
