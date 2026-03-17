import { useState } from 'react';
import {
    Search, Filter, Plus, User,
    MapPin, Shield, ChevronRight, Download, AlertTriangle
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

const StatusBadge = ({ status }: { status: string }) => {
    const styles: any = {
        'Active': 'bg-primary-500/10 text-primary-500 border-primary-500/20',
        'Open': 'bg-primary-500/10 text-primary-500 border-primary-500/20',
        'Pending': 'bg-police-gold/10 text-police-gold border-police-gold/20',
        'Closed': 'bg-police-success/10 text-police-success border-police-success/20',
        'Cold': 'bg-blue-100/10 text-blue-100 border-blue-100/20',
    };
    return (
        <span className={cn("px-3 py-1 text-[10px] font-black uppercase tracking-widest rounded-full border", styles[status] || styles['Active'])}>
            {status}
        </span>
    );
};

const PriorityBadge = ({ priority }: { priority: string }) => {
    const styles: any = {
        'High': 'text-police-crimson font-black',
        'Medium': 'text-police-gold font-black',
        'Low': 'text-blue-400 font-black',
    };
    return (
        <div className="flex items-center gap-1.5">
            <div className={cn("w-1.5 h-1.5 rounded-full animate-pulse",
                priority === 'High' ? 'bg-police-crimson shadow-[0_0_8px_rgba(225,29,72,0.8)]' :
                    priority === 'Medium' ? 'bg-police-gold shadow-[0_0_8px_rgba(251,191,36,0.8)]' : 'bg-blue-400'
            )}></div>
            <span className={cn("text-[10px] uppercase tracking-tighter", styles[priority] || styles['Medium'])}>
                {priority}
            </span>
        </div>
    );
};

const cn = (...inputs: any[]) => inputs.filter(Boolean).join(' ');

export const CaseList = () => {
    const [searchTerm, setSearchTerm] = useState('');

    const mockCases = [
        { id: '1', caseNumber: 'CR/2025/1024', status: 'Active', priority: 'High', type: 'Theft - Commercial', date: '2025-03-12', station: 'Central-4', io: 'Kumar S.' },
        { id: '2', caseNumber: 'CR/2025/1105', status: 'Pending', priority: 'Medium', type: 'Assault', date: '2025-03-14', station: 'North-2', io: 'Sharma A.' },
        { id: '3', caseNumber: 'CR/2025/1118', status: 'Closed', priority: 'Low', type: 'Public Disturbance', date: '2025-03-15', station: 'Central-4', io: 'Prasad V.' },
        { id: '4', caseNumber: 'CR/2025/1242', status: 'Active', priority: 'High', type: 'Murder Investigation', date: '2025-03-16', station: 'East-1', io: 'Yadav R.' },
        { id: '5', caseNumber: 'CR/2025/1288', status: 'Active', priority: 'Medium', type: 'Cyber Fraud', date: '2025-03-17', station: 'Central-4', io: 'Iyer K.' },
    ];

    const filtered = mockCases.filter(c =>
        c.caseNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
        c.type.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-6">
                <div>
                    <h2 className="text-3xl font-black tracking-tight">Case <span className="text-primary-500">Repository</span></h2>
                    <p className="text-blue-100/40 text-sm mt-1 font-medium italic">Showing 128 registered cases in current jurisdiction</p>
                </div>
                <div className="flex flex-wrap items-center gap-3">
                    <button className="btn-secondary">
                        <Filter className="w-4 h-4" />
                        Advance Filter
                    </button>
                    <button className="btn-primary">
                        <Plus className="w-4 h-4" />
                        Launch New FIR
                    </button>
                </div>
            </div>

            <div className="glass-panel border-white/5 overflow-hidden">
                <div className="p-4 border-b border-white/5 bg-white/5 flex flex-col sm:flex-row items-center justify-between gap-4">
                    <div className="relative w-full sm:w-96">
                        <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-blue-100/30" />
                        <input
                            type="text"
                            placeholder="Search by case number, section or officer..."
                            className="input-field pl-12 bg-white/5 border-white/10"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <div className="flex items-center gap-2">
                        <span className="text-[10px] font-bold text-blue-100/20 uppercase tracking-[0.2em] mr-2">Sort by</span>
                        <select className="bg-transparent text-sm font-bold border-none outline-none cursor-pointer">
                            <option>Newest First</option>
                            <option>Priority Level</option>
                            <option>Status Code</option>
                        </select>
                    </div>
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                        <thead>
                            <tr className="bg-white/5 border-b border-white/10">
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40">Reference No</th>
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40">Incident Type</th>
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40">Assigned To</th>
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40 text-center">Priority</th>
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40">Status</th>
                                <th className="px-6 py-4 text-[10px] font-black uppercase tracking-widest text-blue-100/40 text-right">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-white/5">
                            <AnimatePresence mode="popLayout">
                                {filtered.map((c, idx) => (
                                    <motion.tr
                                        layout
                                        initial={{ opacity: 0, y: 10 }}
                                        animate={{ opacity: 1, y: 0 }}
                                        transition={{ delay: idx * 0.05 }}
                                        key={c.id}
                                        className="hover:bg-primary-500/[0.03] group transition-colors cursor-pointer"
                                    >
                                        <td className="px-6 py-5">
                                            <div className="flex items-center gap-3">
                                                <div className="p-2 bg-primary-500/10 rounded-lg group-hover:scale-110 transition-transform">
                                                    <Shield className="w-4 h-4 text-primary-500" />
                                                </div>
                                                <div>
                                                    <p className="text-sm font-black text-white">{c.caseNumber}</p>
                                                    <p className="text-[10px] text-blue-100/30 font-bold uppercase tracking-tighter">Registered 2 days ago</p>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <p className="text-sm font-semibold text-blue-100/80">{c.type}</p>
                                            <div className="flex items-center gap-1.5 mt-1 opacity-40">
                                                <MapPin className="w-3 h-3" />
                                                <span className="text-[10px] font-bold">North Zone Jurisdiction</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <div className="flex items-center gap-2">
                                                <div className="w-7 h-7 rounded-full bg-blue-100/5 flex items-center justify-center border border-white/5">
                                                    <User className="w-4 h-4 text-blue-100/40" />
                                                </div>
                                                <div>
                                                    <p className="text-xs font-bold text-blue-100/70">{c.io}</p>
                                                    <p className="text-[9px] text-blue-100/30 uppercase font-black">Investigating Officer</p>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <div className="flex justify-center">
                                                <PriorityBadge priority={c.priority} />
                                            </div>
                                        </td>
                                        <td className="px-6 py-5">
                                            <StatusBadge status={c.status} />
                                        </td>
                                        <td className="px-6 py-5 text-right">
                                            <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                                                <button className="p-2 hover:bg-white/10 rounded-lg text-blue-100/40 hover:text-white transition-colors">
                                                    <Download className="w-4 h-4" />
                                                </button>
                                                <button className="p-2 hover:bg-white/10 rounded-lg text-blue-100/40 hover:text-white transition-colors">
                                                    <ChevronRight className="w-5 h-5 text-primary-500" />
                                                </button>
                                            </div>
                                        </td>
                                    </motion.tr>
                                ))}
                            </AnimatePresence>
                        </tbody>
                    </table>

                    {filtered.length === 0 && (
                        <div className="p-12 text-center space-y-4">
                            <div className="p-4 bg-white/5 rounded-full w-fit mx-auto border border-white/5">
                                <AlertTriangle className="w-8 h-8 text-police-gold/40" />
                            </div>
                            <div>
                                <p className="text-lg font-bold text-white">No Matching Intel Found</p>
                                <p className="text-sm text-blue-100/30 font-medium">Clear search terms or try advanced filtering for deep directory lookup.</p>
                            </div>
                            <button onClick={() => setSearchTerm('')} className="btn-secondary mx-auto">Reset Directory</button>
                        </div>
                    )}
                </div>

                <div className="p-4 border-t border-white/5 bg-white/5 flex items-center justify-between">
                    <p className="text-[10px] font-black uppercase tracking-widest text-blue-100/20 pl-2">Displaying 1-10 of 128 results</p>
                    <div className="flex items-center gap-1">
                        <button className="p-2 px-4 rounded-lg bg-white/5 text-[10px] font-bold text-blue-100/30 hover:text-white transition-colors">Previous</button>
                        <div className="w-8 h-8 flex items-center justify-center rounded-lg bg-primary-500/10 text-primary-500 border border-primary-500/30 text-xs font-black">1</div>
                        <button className="p-2 px-4 rounded-lg bg-white/5 text-[10px] font-bold text-blue-100/30 hover:text-white transition-colors">Next</button>
                    </div>
                </div>
            </div>
        </div>
    );
};
