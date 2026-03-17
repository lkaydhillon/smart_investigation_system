import React from 'react';
import { motion } from 'framer-motion';
import {
    FileText, Shield, AlertCircle, CheckCircle2,
    TrendingUp, Users, Map, Clock, ArrowUpRight, Search
} from 'lucide-react';
import { useAuthStore } from '../../store/authStore';

const StatCard = ({ title, value, change, icon: Icon, color }: any) => (
    <motion.div
        whileHover={{ y: -5 }}
        className="premium-card"
    >
        <div className="flex justify-between items-start">
            <div className={cn("p-3 rounded-xl bg-opacity-10 border border-opacity-20", color)}>
                <Icon className="w-6 h-6" />
            </div>
            <div className="flex items-center gap-1 text-police-success font-bold text-xs bg-police-success/10 px-2 py-0.5 rounded-full">
                <TrendingUp className="w-3 h-3" />
                {change}%
            </div>
        </div>
        <div className="mt-4">
            <h3 className="text-3xl font-display font-black text-white">{value}</h3>
            <p className="text-sm text-blue-100/40 font-medium uppercase tracking-wider mt-1">{title}</p>
        </div>
    </motion.div>
);

const cn = (...inputs: any[]) => inputs.filter(Boolean).join(' ');

export const Dashboard = () => {
    const { user } = useAuthStore();

    const stats = [
        { title: 'Active Cases', value: '1,284', change: 12, icon: FileText, color: 'bg-primary-500 text-primary-500' },
        { title: 'Investigations', value: '432', change: 5, icon: Shield, color: 'bg-police-gold text-police-gold' },
        { title: 'Urgent Alerts', value: '18', change: -3, icon: AlertCircle, color: 'bg-police-crimson text-police-crimson' },
        { title: 'Cleared Cases', value: '945', change: 8, icon: CheckCircle2, color: 'bg-police-success text-police-success' },
    ];

    return (
        <div className="space-y-8">
            {/* Welcome Header */}
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h1 className="text-4xl font-black tracking-tight flex items-center gap-3">
                        Command <span className="text-primary-500">Center</span>
                    </h1>
                    <p className="text-blue-100/50 font-medium mt-1">
                        Welcome back, <span className="text-white">{user?.fullName}</span>. System status: <span className="text-police-success font-bold">Operational</span>
                    </p>
                </div>
                <div className="flex items-center gap-3">
                    <button className="btn-secondary">
                        <Clock className="w-4 h-4" />
                        Past 30 Days
                    </button>
                    <button className="btn-primary">
                        <Search className="w-4 h-4" />
                        Global Search
                    </button>
                </div>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                {stats.map((stat, idx) => (
                    <StatCard key={idx} {...stat} />
                ))}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

                {/* Main Feed: Case Analytics */}
                <div className="lg:col-span-2 space-y-6">
                    <div className="premium-card p-0">
                        <div className="p-6 border-b border-white/5 flex items-center justify-between">
                            <h3 className="text-xl font-bold flex items-center gap-2">
                                <Map className="w-5 h-5 text-primary-500" />
                                Case Distribution Map
                            </h3>
                            <button className="text-xs font-bold text-primary-500 hover:text-white uppercase tracking-widest transition-colors flex items-center gap-1">
                                View Heatmap <ArrowUpRight className="w-3 h-3" />
                            </button>
                        </div>
                        <div className="h-[400px] w-full bg-police-accent/20 flex items-center justify-center relative overflow-hidden">
                            {/* Simulation map placeholder */}
                            <div className="absolute inset-0 opacity-20 pointer-events-none">
                                <div className="absolute top-1/4 left-1/3 w-4 h-4 bg-police-crimson rounded-full animate-ping"></div>
                                <div className="absolute top-1/2 left-2/3 w-3 h-3 bg-primary-500 rounded-full animate-pulse"></div>
                                <div className="absolute top-2/3 left-1/4 w-3 h-3 bg-police-gold rounded-full animate-pulse"></div>
                            </div>
                            <div className="text-center p-8 glass-panel border-white/10 max-w-xs scale-110">
                                <p className="text-blue-100/40 font-bold uppercase tracking-widest text-[10px] mb-2">Live Intel</p>
                                <p className="text-sm font-medium">Interactive Jurisdictional Map undergoing synchronization...</p>
                            </div>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="premium-card">
                            <h4 className="font-bold mb-4 flex items-center gap-2 text-police-gold">
                                <Users className="w-4 h-4" /> Personnel Tracking
                            </h4>
                            <div className="space-y-4">
                                {[1, 2, 3].map(i => (
                                    <div key={i} className="flex items-center justify-between p-3 rounded-xl bg-white/5 border border-white/5">
                                        <div className="flex items-center gap-3">
                                            <div className="w-8 h-8 rounded-full bg-blue-500/20 flex items-center justify-center text-[10px] font-bold">IO</div>
                                            <div>
                                                <p className="text-xs font-bold">Officer {i}</p>
                                                <p className="text-[10px] text-blue-100/40">Patrol Zone B-4</p>
                                            </div>
                                        </div>
                                        <div className="w-2 h-2 rounded-full bg-police-success"></div>
                                    </div>
                                ))}
                            </div>
                        </div>
                        <div className="premium-card">
                            <h4 className="font-bold mb-4 flex items-center gap-2 text-primary-400">
                                <ArrowUpRight className="w-4 h-4" /> Quick Actions
                            </h4>
                            <div className="grid grid-cols-2 gap-3">
                                <button className="flex flex-col items-center justify-center p-4 rounded-2xl bg-white/5 border border-white/5 hover:bg-primary-500/10 hover:border-primary-500/30 transition-all gap-2 group">
                                    <FileText className="w-6 h-6 text-primary-500" />
                                    <span className="text-[10px] font-bold uppercase tracking-widest text-blue-100/60 transition-colors">Digital FIR</span>
                                </button>
                                <button className="flex flex-col items-center justify-center p-4 rounded-2xl bg-white/5 border border-white/5 hover:bg-primary-500/10 hover:border-primary-500/30 transition-all gap-2 group">
                                    <Map className="w-6 h-6 text-primary-500" />
                                    <span className="text-[10px] font-bold uppercase tracking-widest text-blue-100/60 transition-colors">Scene Entry</span>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Right Sidebar: Timeline / Activity */}
                <div className="space-y-6">
                    <div className="premium-card h-full">
                        <div className="flex items-center justify-between mb-6">
                            <h3 className="font-bold">Recent Intelligence</h3>
                            <div className="p-1 px-2 rounded-md bg-white/5 text-[10px] uppercase font-black text-blue-100/20">Live Feed</div>
                        </div>
                        <div className="space-y-6 border-l-2 border-white/5 ml-2 pl-6">
                            {[
                                { title: 'New Case Registered', time: '12m ago', desc: 'FIR #8282/25 - Theft in Sector-4', color: 'bg-primary-500' },
                                { title: 'Evidence Uploaded', time: '45m ago', desc: 'Digital forensic dump added to Case #4412', color: 'bg-police-success' },
                                { title: 'High Priority Alert', time: '2h ago', desc: 'Suspect entry detected in Geofence Alpha', color: 'bg-police-crimson' },
                                { title: 'Court Order Pending', time: '3h ago', desc: 'Case #2214 requires bail response', color: 'bg-police-gold' },
                                { title: 'IO Assigned', time: '5h ago', desc: 'Insp. Kumar assigned to Cyber Case #99', color: 'bg-blue-400' },
                            ].map((item, idx) => (
                                <div key={idx} className="relative group">
                                    <div className={cn("absolute -left-8 top-1 w-4 h-4 rounded-full border-4 border-police-dark ring-2 ring-white/5", item.color)}></div>
                                    <div className="flex justify-between items-start mb-1">
                                        <h4 className="text-xs font-bold text-white group-hover:text-primary-400 transition-colors">{item.title}</h4>
                                        <span className="text-[10px] text-blue-100/30 font-bold">{item.time}</span>
                                    </div>
                                    <p className="text-xs text-blue-100/50 leading-relaxed">{item.desc}</p>
                                </div>
                            ))}
                        </div>
                        <button className="w-full py-3 mt-8 rounded-xl bg-white/5 hover:bg-white/10 text-xs font-bold uppercase tracking-widest transition-all">
                            Access Intelligence Logs
                        </button>
                    </div>
                </div>

            </div>
        </div>
    );
};
