// @ts-nocheck
import React, { useState } from 'react';
import { View, Text, ScrollView, TouchableOpacity, TextInput, ActivityIndicator, Alert, StyleSheet, StatusBar, FlatList } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { personsApi } from '../api/personsApi';

export const PersonsScreen = () => {
    const insets = useSafeAreaInsets();
    const queryClient = useQueryClient();
    const [searchQuery, setSearchQuery] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [form, setForm] = useState({ fullName: '', fatherName: '', gender: 0, nationality: 'Indian', identificationMarks: '' });

    const resetForm = () => {
        setForm({ fullName: '', fatherName: '', gender: 0, nationality: 'Indian', identificationMarks: '' });
        setEditingId(null);
        setShowForm(false);
    };

    const { data, isLoading, refetch } = useQuery({ queryKey: ['persons'], queryFn: () => personsApi.getAll() });
    const persons = (data?.items || data || []).filter((p: any) => !searchQuery || p.fullName?.toLowerCase().includes(searchQuery.toLowerCase()));

    const createMutation = useMutation({
        mutationFn: personsApi.create,
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['persons'] }); resetForm(); Alert.alert('✅ Success', 'Person record created'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to create person'),
    });

    const updateMutation = useMutation({
        mutationFn: (data: any) => personsApi.update(editingId!, data),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['persons'] }); resetForm(); Alert.alert('✅ Success', 'Person record updated'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to update person'),
    });

    const deleteMutation = useMutation({
        mutationFn: personsApi.delete,
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['persons'] }); Alert.alert('🗑️ Deleted', 'Person record removed'); },
        onError: (e: any) => Alert.alert('Error', e?.response?.data?.error || 'Failed to delete person'),
    });

    const handleSubmit = () => {
        if (!form.fullName.trim()) { Alert.alert('Required', 'Please enter full name'); return; }
        if (editingId) updateMutation.mutate(form);
        else createMutation.mutate(form);
    };

    const handleEdit = (person: any) => {
        setForm({ fullName: person.fullName, fatherName: person.fatherName || '', gender: person.gender || 0, nationality: person.nationality || 'Indian', identificationMarks: person.identificationMarks || '' });
        setEditingId(person.id);
        setShowForm(true);
    };

    const handleDelete = (id: string) => {
        Alert.alert('Confirm Delete', 'Are you sure you want to delete this person record?', [
            { text: 'Cancel', style: 'cancel' },
            { text: 'Delete', style: 'destructive', onPress: () => deleteMutation.mutate(id) }
        ]);
    };

    const genders = ['Male', 'Female', 'Other'];

    return (
        <View style={[s.container, { paddingTop: insets.top }]}>
            <StatusBar barStyle="light-content" backgroundColor="#5B21B6" />
            <View style={s.header}>
                <View style={s.headerRow}>
                    <Text style={s.headerTitle}>👥 Persons Registry</Text>
                    <TouchableOpacity onPress={() => showForm ? resetForm() : setShowForm(true)} style={[s.addBtn, showForm && s.cancelBtn]}>
                        <Text style={[s.addBtnText, showForm && s.cancelText]}>{showForm ? '✕ Cancel' : '+ Add'}</Text>
                    </TouchableOpacity>
                </View>
                <TextInput style={s.searchInput} placeholder="🔍 Search by name..." value={searchQuery} onChangeText={setSearchQuery} placeholderTextColor="rgba(255,255,255,0.5)" />
            </View>

            <ScrollView style={s.body} showsVerticalScrollIndicator={false}>
                {showForm && (
                    <View style={s.formCard}>
                        <Text style={s.formTitle}>{editingId ? 'Edit Person Record' : 'New Person Record'}</Text>
                        <Text style={s.label}>Full Name *</Text>
                        <TextInput style={s.input} placeholder="e.g. Ramesh Kumar Singh" value={form.fullName} onChangeText={t => setForm(p => ({ ...p, fullName: t }))} placeholderTextColor="#94A3B8" />
                        <Text style={s.label}>Father/Husband Name</Text>
                        <TextInput style={s.input} placeholder="Father's name" value={form.fatherName} onChangeText={t => setForm(p => ({ ...p, fatherName: t }))} placeholderTextColor="#94A3B8" />
                        <Text style={s.label}>Gender</Text>
                        <View style={s.genderRow}>
                            {genders.map((g, i) => (
                                <TouchableOpacity key={g} onPress={() => setForm(p => ({ ...p, gender: i }))} style={[s.genderBtn, form.gender === i && s.genderActive]}>
                                    <Text style={[s.genderText, form.gender === i && s.genderActiveText]}>{g}</Text>
                                </TouchableOpacity>
                            ))}
                        </View>
                        <Text style={s.label}>Identification Marks</Text>
                        <TextInput style={s.input} placeholder="Scars, tattoos, distinguishing features..." value={form.identificationMarks} onChangeText={t => setForm(p => ({ ...p, identificationMarks: t }))} placeholderTextColor="#94A3B8" />
                        <TouchableOpacity onPress={handleSubmit} disabled={createMutation.isPending || updateMutation.isPending} style={[s.submitBtn, (createMutation.isPending || updateMutation.isPending) && { opacity: 0.6 }]}>
                            {createMutation.isPending || updateMutation.isPending ? <ActivityIndicator color="#fff" /> : <Text style={s.submitText}>{editingId ? 'Update Record' : 'Create Record'}</Text>}
                        </TouchableOpacity>
                    </View>
                )}

                {isLoading && <ActivityIndicator size="large" color="#7C3AED" style={{ marginTop: 30 }} />}

                {persons.map((item: any) => (
                    <View key={item.id} style={s.personCard}>
                        <View style={s.avatar}><Text style={s.avatarText}>{item.fullName?.[0]?.toUpperCase() || '?'}</Text></View>
                        <View style={{ flex: 1 }}>
                            <Text style={s.personName}>{item.fullName}</Text>
                            <Text style={s.personMeta}>{genders[item.gender] || 'Unknown'} • {item.dateOfBirth ? new Date(item.dateOfBirth).toLocaleDateString('en-IN') : 'DOB N/A'}</Text>
                            {item.identificationMarks && <Text style={s.personMarks} numberOfLines={1}>🔖 {item.identificationMarks}</Text>}
                        </View>
                        <View style={s.actionRow}>
                            <TouchableOpacity onPress={() => handleEdit(item)} style={s.iconBtn}><Text style={{ fontSize: 18 }}>✏️</Text></TouchableOpacity>
                            <TouchableOpacity onPress={() => handleDelete(item.id)} style={s.iconBtn}><Text style={{ fontSize: 18 }}>🗑️</Text></TouchableOpacity>
                        </View>
                    </View>
                ))}

                {!isLoading && persons.length === 0 && !showForm && (
                    <View style={s.emptyState}><Text style={{ fontSize: 48 }}>👥</Text><Text style={s.emptyTitle}>No persons in registry</Text><Text style={s.emptyDesc}>Tap "+ Add" to create a person record (accused, victim, or witness).</Text></View>
                )}
                <View style={{ height: 30 }} />
            </ScrollView>
        </View>
    );
};

const s = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#F8FAFC' },
    header: { backgroundColor: '#5B21B6', paddingHorizontal: 20, paddingTop: 16, paddingBottom: 16 },
    headerRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 },
    headerTitle: { fontSize: 20, fontWeight: '800', color: '#fff' },
    addBtn: { backgroundColor: '#fff', paddingHorizontal: 14, paddingVertical: 8, borderRadius: 8 },
    addBtnText: { color: '#5B21B6', fontWeight: '700', fontSize: 13 },
    cancelBtn: { backgroundColor: 'rgba(255,255,255,0.15)', borderWidth: 1, borderColor: 'rgba(255,255,255,0.3)' },
    cancelText: { color: '#fff' },
    searchInput: { backgroundColor: 'rgba(255,255,255,0.15)', borderRadius: 10, padding: 12, fontSize: 14, color: '#fff' },
    body: { flex: 1, padding: 16 },
    formCard: { backgroundColor: '#fff', borderRadius: 16, padding: 20, marginBottom: 16, borderWidth: 1, borderColor: '#E2E8F0', elevation: 2 },
    formTitle: { fontSize: 18, fontWeight: '700', color: '#0F172A', marginBottom: 16 },
    label: { fontSize: 13, fontWeight: '600', color: '#475569', marginBottom: 6, marginTop: 8 },
    input: { backgroundColor: '#F8FAFC', borderRadius: 10, padding: 14, fontSize: 14, color: '#1E293B', borderWidth: 1, borderColor: '#E2E8F0' },
    genderRow: { flexDirection: 'row', gap: 8 },
    genderBtn: { flex: 1, paddingVertical: 10, borderRadius: 8, backgroundColor: '#F1F5F9', alignItems: 'center', borderWidth: 1, borderColor: '#E2E8F0' },
    genderActive: { backgroundColor: '#EDE9FE', borderColor: '#8B5CF6' },
    genderText: { fontSize: 13, fontWeight: '600', color: '#64748B' },
    genderActiveText: { color: '#6D28D9' },
    submitBtn: { backgroundColor: '#5B21B6', padding: 16, borderRadius: 12, alignItems: 'center', marginTop: 16 },
    submitText: { color: '#fff', fontWeight: '700', fontSize: 16 },
    personCard: { backgroundColor: '#fff', borderRadius: 14, padding: 14, marginBottom: 8, borderWidth: 1, borderColor: '#E2E8F0', flexDirection: 'row', alignItems: 'center' },
    avatar: { width: 48, height: 48, borderRadius: 14, backgroundColor: '#EDE9FE', justifyContent: 'center', alignItems: 'center', marginRight: 14 },
    avatarText: { fontSize: 20, fontWeight: '800', color: '#7C3AED' },
    personName: { fontSize: 15, fontWeight: '700', color: '#0F172A' },
    personMeta: { fontSize: 12, color: '#64748B', marginTop: 2 },
    personMarks: { fontSize: 11, color: '#94A3B8', marginTop: 2 },
    actionRow: { flexDirection: 'row', gap: 6, marginLeft: 8 },
    iconBtn: { padding: 8, backgroundColor: '#F1F5F9', borderRadius: 8 },
    emptyState: { alignItems: 'center', marginTop: 60 },
    emptyTitle: { fontSize: 18, fontWeight: '700', color: '#334155', marginTop: 12 },
    emptyDesc: { fontSize: 14, color: '#94A3B8', textAlign: 'center', marginTop: 8, paddingHorizontal: 32 },
});
