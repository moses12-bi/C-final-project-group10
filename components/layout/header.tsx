'use client';

import { Bell, Search, Sun, Moon } from 'lucide-react';
import { useTheme } from '@/contexts/theme-context';

export default function Header() {
    const { theme, toggleTheme } = useTheme();

    return (
        <header className="sticky top-0 z-10 bg-dark-900/80 backdrop-blur-md border-b border-dark-800">
            <div className="flex items-center justify-between px-6 py-4">
                {/* Search */}
                <div className="flex-1 max-w-2xl">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
                        <input
                            type="text"
                            placeholder="Search projects, tasks, or team members..."
                            className="w-full pl-10 pr-4 py-2 bg-dark-950/50 border border-dark-700 rounded-lg focus:outline-none focus:ring-1 focus:ring-primary-500 text-slate-200 placeholder:text-slate-500 transition-all focus:bg-dark-950"
                        />
                    </div>
                </div>

                {/* Right Section */}
                <div className="flex items-center gap-4 ml-4">
                    {/* Theme Toggle (Optional now since we enforce dark, but kept for logic) */}
                    <button
                        onClick={toggleTheme}
                        className="p-2 rounded-lg hover:bg-dark-800 transition-colors"
                        aria-label="Toggle theme"
                    >
                        {theme === 'dark' ? (
                            <Sun className="w-5 h-5 text-slate-400 hover:text-yellow-400" />
                        ) : (
                            <Moon className="w-5 h-5 text-slate-400" />
                        )}
                    </button>

                    {/* Notifications */}
                    <button
                        className="relative p-2 rounded-lg hover:bg-dark-800 transition-colors"
                        aria-label="Notifications"
                    >
                        <Bell className="w-5 h-5 text-slate-400 hover:text-slate-200" />
                        <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full animate-pulse"></span>
                    </button>
                </div>
            </div>
        </header>
    );
}
