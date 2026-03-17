import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Shield, Fingerprint, Lock, Mail, ArrowRight, UserCheck, ShieldCheck, User } from 'lucide-react';
import { motion } from 'framer-motion';
import { authApi } from '../../services/authApi';
import { type UserRole } from '../../store/authStore';

const QuickLoginCard = ({ role, title, desc, onClick, icon: Icon }: any) => (
    <button
        type="button"
        onClick={() => onClick(role)}
        className="group flex flex-col items-center p-6 glass-panel hover:bg-white/10 hover:border-primary-500/30 transition-all duration-300 w-full text-center hover:scale-[1.02] border border-white/5"
    >
        <div className={cn(
            "p-4 rounded-2xl mb-4 group-hover:scale-110 transition-transform duration-300 border border-white/5 shadow-xl",
            role === 'SuperAdmin' ? "bg-police-gold/10 text-police-gold" :
                role === 'Admin' ? "bg-primary-500/10 text-primary-500" : "bg-blue-400/10 text-blue-400"
        )}>
            <Icon className="w-8 h-8" />
        </div>
        <h3 className="font-display font-bold text-lg mb-1">{title}</h3>
        <p className="text-[10px] text-blue-100/50 uppercase tracking-widest font-bold font-sans">{desc}</p>
    </button>
);

const cn = (...inputs: any[]) => inputs.filter(Boolean).join(' ');

export const LoginPage = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [credentials, setCredentials] = useState({ username: '', password: '' });

    const handleQuickLogin = async (role: UserRole) => {
        setLoading(true);
        setError(null);
        try {
            const testCreds = {
                SuperAdmin: { username: 'superadmin', password: 'Password@123' },
                Admin: { username: 'pstation_sho', password: 'Password@123' },
                Investigator: { username: 'investigator_1', password: 'Password@123' }
            };

            const creds = testCreds[role];
            await authApi.login(creds);
            navigate('/');
        } catch (err: any) {
            setError(err.toString());
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            await authApi.login(credentials);
            navigate('/');
        } catch (err: any) {
            setError(err.toString());
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-police-dark flex items-center justify-center p-4 selection:bg-primary-500/40 relative overflow-hidden">
            <div className="absolute top-0 left-0 w-full h-full pointer-events-none opacity-20">
                <div className="absolute top-[10%] left-[10%] w-96 h-96 bg-primary-600 rounded-full blur-[120px] mix-blend-screen animate-pulse-slow"></div>
                <div className="absolute bottom-[10%] right-[10%] w-96 h-96 bg-police-crimson rounded-full blur-[120px] mix-blend-screen opacity-50 animate-pulse-slow"></div>
            </div>

            <div className="w-full max-w-6xl grid grid-cols-1 lg:grid-cols-2 gap-12 items-center relative z-10">
                <div className="hidden lg:block space-y-8">
                    <motion.div
                        initial={{ opacity: 0, scale: 0.9 }}
                        animate={{ opacity: 1, scale: 1 }}
                        className="flex items-center gap-4 py-2"
                    >
                        <Shield className="w-16 h-16 text-primary-500 fill-primary-500/10 drop-shadow-[0_0_15px_rgba(14,165,233,0.3)]" />
                        <div>
                            <h1 className="text-4xl font-display font-black tracking-tight mb-0">
                                SMART<span className="text-primary-500">INVESTIGATION</span>
                            </h1>
                            <p className="text-blue-200/50 uppercase tracking-[0.2em] font-bold text-xs">Law Enforcement Suite v2.1</p>
                        </div>
                    </motion.div>

                    <motion.div
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.2 }}
                        className="space-y-6"
                    >
                        <h2 className="text-5xl font-display font-bold leading-tight text-white max-w-md">
                            Mission Critical <span className="text-gradient">Case Intelligence.</span>
                        </h2>
                        <p className="text-lg text-blue-100/60 font-medium max-w-sm">
                            Integrated platform for investigation tracking, AI-powered forensics, and legal compliance.
                        </p>
                    </motion.div>

                    <div className="grid grid-cols-2 gap-4 max-w-md pt-4">
                        <div className="p-4 glass-panel border-white/5">
                            <p className="text-2xl font-bold text-white mb-0">70+</p>
                            <p className="text-xs text-blue-100/40 font-bold uppercase tracking-widest mt-1">Data Entities</p>
                        </div>
                        <div className="p-4 glass-panel border-white/5">
                            <p className="text-2xl font-bold text-white mb-0">Live</p>
                            <p className="text-xs text-blue-100/40 font-bold uppercase tracking-widest mt-1">Forensics Integration</p>
                        </div>
                    </div>
                </div>

                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    className="glass-panel p-8 md:p-12 border-white/10 shadow-[0_0_50px_rgba(0,0,0,0.5)] bg-slate-900/40 backdrop-blur-2xl"
                >
                    <div className="mb-8">
                        <h3 className="text-3xl font-display font-black mb-2">Internal Access</h3>
                        <p className="text-blue-100/50 font-medium">Please enter your authorized credentials or use quick login.</p>
                    </div>

                    {error && (
                        <motion.div
                            initial={{ opacity: 0, x: -10 }}
                            animate={{ opacity: 1, x: 0 }}
                            className="mb-6 p-4 bg-red-500/10 border border-red-500/20 rounded-xl flex items-center gap-3 text-red-500 font-bold text-sm"
                        >
                            <div className="p-1.5 bg-red-500 rounded-lg text-white">
                                <Lock className="w-4 h-4" />
                            </div>
                            {error}
                        </motion.div>
                    )}

                    <form onSubmit={handleSubmit} className="space-y-5">
                        <div className="space-y-2">
                            <label className="text-xs font-bold uppercase tracking-widest text-blue-100/40 pl-1">Username</label>
                            <div className="relative group">
                                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-blue-100/20 group-focus-within:text-primary-500 transition-colors" />
                                <input
                                    type="text"
                                    value={credentials.username}
                                    onChange={(e) => setCredentials(prev => ({ ...prev, username: e.target.value }))}
                                    required
                                    placeholder="Enter badge id or email"
                                    className="input-field pl-12"
                                />
                            </div>
                        </div>

                        <div className="space-y-2">
                            <label className="text-xs font-bold uppercase tracking-widest text-blue-100/40 pl-1">Secret Key</label>
                            <div className="relative group">
                                <Fingerprint className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-blue-100/20 group-focus-within:text-primary-500 transition-colors" />
                                <input
                                    type="password"
                                    value={credentials.password}
                                    onChange={(e) => setCredentials(prev => ({ ...prev, password: e.target.value }))}
                                    required
                                    placeholder="••••••••"
                                    className="input-field pl-12"
                                />
                            </div>
                        </div>

                        <button
                            type="submit"
                            disabled={loading}
                            className="btn-primary w-full py-4 text-lg mt-2 relative overflow-hidden group active:scale-[0.98] transition-all"
                        >
                            <span className="relative z-10 flex items-center justify-center gap-2">
                                {loading ? 'Authenticating...' : 'Sign In Now'}
                                {!loading && <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />}
                            </span>
                            <div className="absolute inset-0 bg-gradient-to-r from-primary-400 to-primary-600 opacity-0 group-hover:opacity-10 scale-150 transition-all duration-500 rounded-full blur-3xl"></div>
                        </button>
                    </form>

                    <div className="mt-12 flex flex-col items-center">
                        <div className="w-full flex items-center gap-4 mb-8">
                            <div className="h-px flex-1 bg-white/5"></div>
                            <span className="text-[10px] font-black uppercase tracking-[0.2em] text-blue-100/20">Authorized Roles</span>
                            <div className="h-px flex-1 bg-white/5"></div>
                        </div>

                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 w-full">
                            <QuickLoginCard
                                role="SuperAdmin"
                                title="Super Admin"
                                desc="SP / HQ Level"
                                icon={ShieldCheck}
                                onClick={handleQuickLogin}
                            />
                            <QuickLoginCard
                                role="Admin"
                                title="Station Head"
                                desc="SHO / Admin"
                                icon={UserCheck}
                                onClick={handleQuickLogin}
                            />
                            <QuickLoginCard
                                role="Investigator"
                                title="Investigator"
                                desc="IO / Inspector"
                                icon={User}
                                onClick={handleQuickLogin}
                            />
                        </div>
                    </div>
                </motion.div>
            </div>
        </div>
    );
};
