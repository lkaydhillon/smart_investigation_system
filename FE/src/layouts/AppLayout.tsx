import React, { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import {
    LayoutDashboard, FileText, Shield, User, LogOut, Search, Bell,
    Settings, Menu, X, Landmark, Gavel, Cpu, HardDrive
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useAuthStore } from '../store/authStore';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

const Navbar = ({ onOpenSidebar }: { onOpenSidebar: () => void }) => {
    const { user } = useAuthStore();

    return (
        <nav className="fixed top-0 left-0 right-0 h-16 bg-police-dark/80 backdrop-blur-md border-b border-white/5 z-50 flex items-center justify-between px-4 md:px-8">
            <div className="flex items-center gap-3">
                <button className="md:hidden p-2 hover:bg-white/5 rounded-lg" onClick={onOpenSidebar}>
                    <Menu className="w-6 h-6" />
                </button>
                <div className="flex items-center gap-2">
                    <Shield className="w-8 h-8 text-primary-500 fill-primary-500/10" />
                    <span className="font-display font-bold text-xl tracking-tight hidden sm:block">
                        SMART<span className="text-primary-500 italic">INV</span>
                    </span>
                </div>
            </div>

            <div className="flex items-center gap-4">
                <div className="hidden lg:flex items-center bg-white/5 rounded-full px-4 py-1.5 border border-white/5 w-64">
                    <Search className="w-4 h-4 text-blue-100/30" />
                    <input
                        type="text"
                        placeholder="Search cases or FIRs..."
                        className="bg-transparent border-none outline-none text-sm px-2 w-full placeholder-blue-100/30"
                    />
                </div>

                <button className="p-2 relative hover:bg-white/5 rounded-full transition-colors">
                    <Bell className="w-5 h-5 text-blue-100/70" />
                    <span className="absolute top-1 right-1 w-2 h-2 bg-police-crimson rounded-full border-2 border-police-dark"></span>
                </button>

                <div className="flex items-center gap-3 pl-2 border-l border-white/5">
                    <div className="text-right hidden sm:block">
                        <p className="text-sm font-semibold leading-tight">{user?.fullName}</p>
                        <p className="text-[10px] text-primary-400 font-bold uppercase tracking-widest">{user?.role}</p>
                    </div>
                    <div className="w-10 h-10 rounded-full bg-primary-500/10 border border-primary-500/30 flex items-center justify-center">
                        <User className="w-6 h-6 text-primary-500" />
                    </div>
                </div>
            </div>
        </nav>
    );
};

const Sidebar = ({ isOpen, onClose }: { isOpen: boolean; onClose: () => void }) => {
    const navigate = useNavigate();
    const location = useLocation();
    const logout = useAuthStore(state => state.logout);
    const userRole = useAuthStore(state => state.user?.role);

    const menuItems = [
        { name: 'Dashboard', icon: LayoutDashboard, path: '/', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
        { name: 'My Cases', icon: FileText, path: '/cases', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
        { name: 'Investigation', icon: HardDrive, path: '/investigation', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
        { name: 'Legal Sections', icon: Landmark, path: '/legal', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
        { name: 'Court Affairs', icon: Gavel, path: '/court', roles: ['Admin', 'SuperAdmin'] },
        { name: 'AI Assistant', icon: Cpu, path: '/ai', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
        { name: 'Administration', icon: Shield, path: '/admin', roles: ['Admin', 'SuperAdmin'] },
        { name: 'Settings', icon: Settings, path: '/settings', roles: ['Investigator', 'Admin', 'SuperAdmin'] },
    ];

    const filteredMenu = menuItems.filter(item => item.roles.includes(userRole as any));

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <>
            {/* Mobile Overlay */}
            <AnimatePresence>
                {isOpen && (
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={onClose}
                        className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 md:hidden"
                    />
                )}
            </AnimatePresence>

            <aside className={cn(
                "fixed left-0 top-16 h-[calc(100vh-64px)] w-64 bg-police-dark/95 border-r border-white/5 z-50 transition-transform duration-300 md:translate-x-0 overflow-y-auto",
                !isOpen && "-translate-x-full"
            )}>
                <div className="p-6 flex flex-col h-full">
                    <div className="flex-1 space-y-1.5">
                        {filteredMenu.map((item) => {
                            const isActive = location.pathname === item.path;
                            return (
                                <Link
                                    key={item.path}
                                    to={item.path}
                                    onClick={() => onClose()}
                                    className={cn(
                                        "flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 group relative",
                                        isActive
                                            ? "bg-primary-500/10 text-primary-500 border border-primary-500/20"
                                            : "text-blue-100/60 hover:text-white hover:bg-white/5 border border-transparent"
                                    )}
                                >
                                    <item.icon className={cn("w-5 h-5", isActive ? "text-primary-500" : "text-blue-100/40 group-hover:text-blue-100")} />
                                    <span className="font-medium text-sm">{item.name}</span>
                                    {isActive && <motion.div layoutId="sidebar-active" className="absolute left-0 w-1 h-3/5 bg-primary-500 rounded-r-md" />}
                                </Link>
                            );
                        })}
                    </div>

                    <button
                        onClick={handleLogout}
                        className="flex items-center gap-3 px-4 py-3 text-police-crimson hover:bg-police-crimson/10 rounded-xl transition-all font-semibold mt-8 mb-4 border border-transparent"
                    >
                        <LogOut className="w-5 h-5" />
                        <span className="text-sm">Log Out</span>
                    </button>
                </div>
            </aside>
        </>
    );
};

export const AppLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [sidebarOpen, setSidebarOpen] = useState(false);

    return (
        <div className="min-h-screen bg-police-dark glow-mesh">
            <Navbar onOpenSidebar={() => setSidebarOpen(true)} />
            <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />

            <main className="md:pl-64 pt-16 min-h-screen">
                <div className="page-container animate-entrance">
                    {children}
                </div>
            </main>
        </div>
    );
};
