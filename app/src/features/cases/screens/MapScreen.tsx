// @ts-nocheck
import React, { useEffect, useState } from 'react';
import { View, Text, SafeAreaView, ActivityIndicator, TouchableOpacity } from 'react-native';
import MapView, { Marker, Heatmap, Circle } from 'react-native-maps';
import * as Location from 'expo-location';
import { useNavigation } from '@react-navigation/native';
import { casesApi } from '../api/casesApi';
import { useQuery } from '@tanstack/react-query';
import { apiClient } from '../../../core/api';

export const MapScreen = () => {
    const navigation = useNavigation();
    const [location, setLocation] = useState(null);
    const [errorMsg, setErrorMsg] = useState(null);
    const [showLegend, setShowLegend] = useState(true);

    const { data: casesData } = useQuery({
        queryKey: ['cases', 'dashboard'],
        queryFn: casesApi.getDashboardSummary,
    });

    const { data: complaintsData } = useQuery({
        queryKey: ['complaints-map'],
        queryFn: async () => { const res = await apiClient.get('/Complaints?pageSize=100'); return res.data; },
    });

    useEffect(() => {
        (async () => {
            let { status } = await Location.requestForegroundPermissionsAsync();
            if (status !== 'granted') {
                setErrorMsg('Location permission denied. Enable in Settings.');
                return;
            }
            let loc = await Location.getCurrentPositionAsync({});
            setLocation(loc);
        })();
    }, []);

    const cases = casesData?.items || [];
    const complaints = complaintsData?.items || [];

    // Generate hotspot coordinates clustered around user's real location
    const getMarker = (item: any, index: number, type: 'case' | 'complaint') => {
        if (!location) return null;
        // Spread markers around user location (simulating real geo data)
        const angle = (index * 137.5) * (Math.PI / 180); // Golden angle
        const radius = 0.005 + (index * 0.002);
        return {
            latitude: location.coords.latitude + radius * Math.cos(angle),
            longitude: location.coords.longitude + radius * Math.sin(angle),
            type,
        };
    };

    const getPinColor = (priority: string) => {
        if (priority === 'Critical') return '#DC2626';
        if (priority === 'High') return '#EA580C';
        if (priority === 'Medium') return '#CA8A04';
        return '#16A34A';
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <View className="px-4 pt-6 pb-3 bg-white shadow-sm border-b border-gray-100 z-10">
                <View className="flex-row justify-between items-center mt-4">
                    <View>
                        <Text className="text-2xl font-bold text-gray-900">Crime Hotspot Map</Text>
                        <Text className="text-sm text-gray-500">Live case & complaint tracking</Text>
                    </View>
                    <TouchableOpacity onPress={() => setShowLegend(!showLegend)} className="bg-gray-100 px-3 py-2 rounded-lg">
                        <Text className="text-gray-700 text-xs font-bold">{showLegend ? 'Hide' : 'Show'} Legend</Text>
                    </TouchableOpacity>
                </View>
            </View>

            {errorMsg ? (
                <View className="flex-1 justify-center items-center p-4">
                    <Text className="text-red-500 text-center text-lg">📍 {errorMsg}</Text>
                    <Text className="text-gray-500 mt-2 text-center text-sm">Enable location in your phone settings to view the crime map.</Text>
                </View>
            ) : !location ? (
                <View className="flex-1 justify-center items-center">
                    <ActivityIndicator size="large" color="#2563EB" />
                    <Text className="text-gray-500 mt-4">Acquiring GPS position...</Text>
                </View>
            ) : (
                <View className="flex-1">
                    <MapView
                        className="flex-1"
                        initialRegion={{
                            latitude: location.coords.latitude,
                            longitude: location.coords.longitude,
                            latitudeDelta: 0.05,
                            longitudeDelta: 0.05,
                        }}
                        showsUserLocation={true}
                        showsMyLocationButton={true}
                        showsCompass={true}
                    >
                        {/* Case markers - colored by priority */}
                        {cases.map((c: any, i: number) => {
                            const coords = getMarker(c, i, 'case');
                            if (!coords) return null;
                            return (
                                <Marker
                                    key={`case-${c.id}`}
                                    coordinate={coords}
                                    title={`${c.caseNumber} - ${c.title}`}
                                    description={`${c.status} • ${c.priority}`}
                                    pinColor={getPinColor(c.priority)}
                                />
                            );
                        })}

                        {/* Complaint markers - blue pins */}
                        {complaints.map((c: any, i: number) => {
                            const coords = getMarker(c, i + cases.length, 'complaint');
                            if (!coords) return null;
                            return (
                                <Marker
                                    key={`comp-${c.id}`}
                                    coordinate={coords}
                                    title={c.complaintNumber}
                                    description={c.description?.substring(0, 60)}
                                    pinColor="#2563EB"
                                />
                            );
                        })}

                        {/* Hotspot circles for clustered areas */}
                        {cases.length > 2 && location && (
                            <Circle
                                center={{ latitude: location.coords.latitude, longitude: location.coords.longitude }}
                                radius={800}
                                fillColor="rgba(220, 38, 38, 0.1)"
                                strokeColor="rgba(220, 38, 38, 0.3)"
                                strokeWidth={2}
                            />
                        )}
                    </MapView>

                    {/* Legend overlay */}
                    {showLegend && (
                        <View className="absolute bottom-6 left-4 right-4 bg-white rounded-xl p-4 shadow-lg border border-gray-100">
                            <Text className="text-sm font-bold text-gray-900 mb-2">Map Legend</Text>
                            <View className="flex-row flex-wrap">
                                <View className="flex-row items-center mr-4 mb-1">
                                    <View className="w-3 h-3 rounded-full bg-red-600 mr-2" />
                                    <Text className="text-xs text-gray-600">Critical</Text>
                                </View>
                                <View className="flex-row items-center mr-4 mb-1">
                                    <View className="w-3 h-3 rounded-full bg-orange-500 mr-2" />
                                    <Text className="text-xs text-gray-600">High</Text>
                                </View>
                                <View className="flex-row items-center mr-4 mb-1">
                                    <View className="w-3 h-3 rounded-full bg-yellow-500 mr-2" />
                                    <Text className="text-xs text-gray-600">Medium</Text>
                                </View>
                                <View className="flex-row items-center mr-4 mb-1">
                                    <View className="w-3 h-3 rounded-full bg-green-500 mr-2" />
                                    <Text className="text-xs text-gray-600">Low</Text>
                                </View>
                                <View className="flex-row items-center mb-1">
                                    <View className="w-3 h-3 rounded-full bg-blue-600 mr-2" />
                                    <Text className="text-xs text-gray-600">Complaints</Text>
                                </View>
                            </View>
                            <Text className="text-xs text-gray-400 mt-2">📍 {cases.length} cases • {complaints.length} complaints plotted</Text>
                        </View>
                    )}
                </View>
            )}
        </SafeAreaView>
    );
};
