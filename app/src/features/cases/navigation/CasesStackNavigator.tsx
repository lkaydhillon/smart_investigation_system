import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { CaseListScreen } from '../screens/CaseListScreen';
import { CaseDetailScreen } from '../screens/CaseDetailScreen';

export type CasesStackParamList = {
    CaseList: undefined;
    CaseDetail: { caseId: string };
};

const Stack = createNativeStackNavigator<CasesStackParamList>();

export const CasesStackNavigator = () => {
    return (
        <Stack.Navigator screenOptions={{ headerShown: false }}>
            <Stack.Screen name="CaseList" component={CaseListScreen} />
            <Stack.Screen name="CaseDetail" component={CaseDetailScreen} />
        </Stack.Navigator>
    );
};
