import React, { useState, useRef, useEffect } from 'react';
import {
    Send, Cpu, MessageSquare, History, BookOpen,
    ChevronRight, ArrowRight, CornerDownLeft, Sparkles, Trash2, ShieldCheck, Download, Copy
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

const Message = ({ text, sender, time }: { text: string; sender: 'user' | 'ai'; time: string }) => {
    const isAI = sender === 'ai';
    return (
        <motion.div
            initial={{ opacity: 0, y: 10, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            className={cn("flex flex-col gap-2 max-w-[85%] mb-6", isAI ? "self-start items-start" : "self-end items-end ml-auto")}
        >
            <div className={cn(
                "p-5 rounded-3xl text-sm font-medium leading-relaxed relative group",
                isAI
                    ? "bg-police-accent border border-white/5 rounded-tl-none shadow-xl text-blue-50/90"
                    : "bg-primary-600 border border-primary-500 rounded-tr-none shadow-primary-500/20 text-white"
            )}
            >
                <div className={cn("flex items-center gap-2 mb-2 pb-2 border-b border-white/5", isAI ? "text-primary-400" : "text-white/50")}>
                    {isAI ? <Cpu className="w-3.5 h-3.5" /> : <ShieldCheck className="w-3.5 h-3.5" />}
                    <span className="text-[10px] uppercase font-black tracking-widest leading-none">{isAI ? 'Neural Legal Unit' : 'Authorized Access'}</span>
                </div>
                <p className="whitespace-pre-wrap">{text}</p>

                {isAI && (
                    <div className="absolute top-1/2 -right-12 translate-y-[-50%] flex flex-col gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button className="p-1.5 hover:bg-white/10 rounded-lg bg-white/5 border border-white/5"><Copy className="w-3.5 h-3.5" /></button>
                        <button className="p-1.5 hover:bg-white/10 rounded-lg bg-white/5 border border-white/5"><Download className="w-3.5 h-3.5" /></button>
                    </div>
                )}
            </div>
            <span className="text-[10px] font-black uppercase tracking-widest text-blue-100/20 px-4">{time}</span>
        </motion.div>
    );
};

const cn = (...inputs: any[]) => inputs.filter(Boolean).join(' ');

export const AIAssistant = () => {
    const [input, setInput] = useState('');
    const [messages, setMessages] = useState([
        { id: '1', text: "Ready. I have indexed the latest IPC, BNS, and BNSS updates. How can I assist with your investigation today?", sender: 'ai', time: 'SYSTEM START' },
    ]);
    const [isTyping, setIsTyping] = useState(false);
    const scrollRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [messages, isTyping]);

    const handleSend = async (e?: React.FormEvent) => {
        e?.preventDefault();
        if (!input.trim() || isTyping) return;

        const userMsg = { id: Date.now().toString(), text: input, sender: 'user', time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) };
        setMessages(prev => [...prev, userMsg as any]);
        setInput('');
        setIsTyping(true);

        // AI Response Simulation
        setTimeout(() => {
            const aiMsg = {
                id: (Date.now() + 1).toString(),
                text: "Analyzing incident parameters... \n\nBased on your query, Sections 302 and 34 of the Indian Penal Code (IPC) may apply here. \n\nKey Procedural Requirement: \n- BNSS 105: Videography of search/seizure is mandatory. \n- BNSS 173: Zero FIR must be transferred within 24 hours.",
                sender: 'ai',
                time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
            };
            setMessages(prev => [...prev, aiMsg as any]);
            setIsTyping(false);
        }, 1500);
    };

    const prompts = [
        "What are my requirements for Zero FIR under BNSS?",
        "Generate a digital search memorandum template",
        "Find relevant sections for financial digital fraud",
        "Outline custody procedure for juvenile suspects"
    ];

    return (
        <div className="h-[calc(100vh-140px)] flex flex-col gap-6">

            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-black tracking-tight flex items-center gap-3">
                        AI <span className="text-primary-500">Legal Core</span>
                    </h2>
                    <p className="text-blue-100/40 text-sm mt-1 font-medium font-sans">Real-time investigative support with neural legal cross-referencing</p>
                </div>
                <div className="flex items-center gap-3">
                    <button className="btn-secondary text-[10px] uppercase font-black p-2 px-4 border-white/5 shadow-xl">
                        <History className="w-4 h-4" />
                        Investigation Logs
                    </button>
                    <button className="btn-secondary text-police-crimson/80 border-police-crimson/10 p-2 rounded-xl" onClick={() => setMessages(messages.slice(0, 1))}>
                        <Trash2 className="w-4 h-4" />
                    </button>
                </div>
            </div>

            <div className="flex-1 flex flex-col lg:flex-row gap-6 overflow-hidden">

                {/* Chat Interface */}
                <div className="flex-1 flex flex-col glass-panel border-white/10 overflow-hidden shadow-[0_30px_60px_-15px_rgba(0,0,0,0.5)] bg-slate-900/40 relative">

                    {/* Messages Area */}
                    <div ref={scrollRef} className="flex-1 overflow-y-auto p-6 flex flex-col scroll-smooth">
                        <AnimatePresence>
                            {messages.map((m) => (
                                <Message key={m.id} {...m as any} />
                            ))}
                            {isTyping && (
                                <div className="flex flex-col gap-2 max-w-[85%] self-start animate-pulse">
                                    <div className="p-5 rounded-3xl bg-police-accent border border-white/5 rounded-tl-none flex gap-2 items-center">
                                        <div className="w-1.5 h-1.5 bg-primary-500 rounded-full animate-bounce [animation-delay:-0.3s]"></div>
                                        <div className="w-1.5 h-1.5 bg-primary-500 rounded-full animate-bounce [animation-delay:-0.15s]"></div>
                                        <div className="w-1.5 h-1.5 bg-primary-500 rounded-full animate-bounce"></div>
                                    </div>
                                </div>
                            )}
                        </AnimatePresence>
                    </div>

                    {/* Quick Prompts */}
                    <div className="px-6 py-4 flex gap-2 overflow-x-auto no-scrollbar border-t border-white/5 shrink-0 bg-white/[0.02]">
                        {prompts.map((p, i) => (
                            <button
                                key={i}
                                onClick={() => setInput(p)}
                                className="px-4 py-2 bg-white/5 border border-white/5 whitespace-nowrap text-[10px] font-black uppercase tracking-widest rounded-xl hover:bg-white/10 transition-all hover:border-primary-500/30 text-blue-100/50 hover:text-white"
                            >
                                {p}
                            </button>
                        ))}
                    </div>

                    {/* Input Area */}
                    <form onSubmit={handleSend} className="p-6 pt-0 shrink-0">
                        <div className="relative group p-0.5 rounded-2xl bg-gradient-to-tr from-white/10 to-transparent focus-within:from-primary-500/50 shadow-2xl transition-all duration-500">
                            <div className="bg-police-dark rounded-[14px] flex items-center pr-4">
                                <input
                                    type="text"
                                    value={input}
                                    onChange={(e) => setInput(e.target.value)}
                                    placeholder="Describe crime parameters or ask legal procedures..."
                                    className="flex-1 bg-transparent border-none outline-none p-4 px-6 text-sm placeholder-blue-100/20"
                                />
                                <div className="flex items-center gap-2">
                                    <button type="button" className="p-2 text-blue-100/30 hover:text-white transition-colors">
                                        <Sparkles className="w-5 h-5" />
                                    </button>
                                    <button
                                        type="submit"
                                        disabled={!input.trim()}
                                        className="p-2 bg-primary-600 rounded-xl text-white hover:bg-primary-500 disabled:opacity-30 disabled:grayscale transition-all shadow-lg hover:shadow-primary-500/30"
                                    >
                                        <CornerDownLeft className="w-5 h-5" />
                                    </button>
                                </div>
                            </div>
                        </div>
                        <p className="text-center text-[9px] font-bold text-blue-100/10 uppercase tracking-[0.3em] mt-4">Authorized Investigation Support Agent (Ver. 9.4.1)</p>
                    </form>
                </div>

                {/* Knowledge Sidebar - Desktop Only */}
                <div className="hidden lg:flex flex-col w-80 gap-6">
                    <div className="premium-card bg-police-accent/40 flex-1">
                        <div className="flex items-center gap-2 mb-6 p-2 rounded-xl bg-primary-500/10 border border-primary-500/20 w-fit">
                            <BookOpen className="w-4 h-4 text-primary-500" />
                            <span className="text-[10px] font-black uppercase tracking-widest text-primary-500">Legal Library</span>
                        </div>
                        <h4 className="font-display font-bold text-lg mb-2">Neural Cross-Index</h4>
                        <p className="text-xs text-blue-100/40 font-medium leading-relaxed mb-6">Real-time matching between reported observations and established statutes.</p>

                        <div className="space-y-4">
                            {[
                                { title: 'BNS (New IPC)', progress: 100, color: 'bg-primary-500' },
                                { title: 'BNSS (New CrPC)', progress: 95, color: 'bg-police-gold' },
                                { title: 'BSA (New Evidence)', progress: 80, color: 'bg-police-success' }
                            ].map((lib, i) => (
                                <div key={i} className="space-y-2">
                                    <div className="flex justify-between items-center text-[10px] font-black uppercase tracking-widest">
                                        <span className="text-blue-100/60">{lib.title}</span>
                                        <span className="text-white/60">{lib.progress}%</span>
                                    </div>
                                    <div className="h-1.5 w-full bg-white/5 rounded-full overflow-hidden border border-white/5">
                                        <motion.div
                                            initial={{ width: 0 }}
                                            animate={{ width: `${lib.progress}%` }}
                                            className={cn("h-full rounded-full shadow-[0_0_8px_rgba(255,255,255,0.1)]", lib.color)}
                                        ></motion.div>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <div className="mt-8 p-4 bg-white/5 rounded-2xl border border-white/5">
                            <p className="text-[10px] font-black text-blue-100/40 uppercase mb-3">Suggested Procedure</p>
                            <div className="flex items-center gap-2 text-white hover:text-primary-400 cursor-pointer transition-colors group">
                                <ArrowRight className="w-3.5 h-3.5 group-hover:translate-x-1 transition-transform" />
                                <span className="text-xs font-bold leading-tight">Digital Seizure Protocol for Encrypted Storage</span>
                            </div>
                        </div>
                    </div>

                    <div className="premium-card bg-gradient-to-br from-primary-600/20 to-transparent border-primary-500/20">
                        <div className="flex items-center gap-2 mb-3">
                            <ShieldCheck className="w-5 h-5 text-primary-400" />
                            <h4 className="text-sm font-bold">Audit Assurance</h4>
                        </div>
                        <p className="text-[10px] leading-relaxed text-blue-100/60 font-medium font-sans">
                            Every interaction is cryptographically hashed and logged for forensic audit compliance.
                        </p>
                    </div>
                </div>

            </div>
        </div>
    );
};
