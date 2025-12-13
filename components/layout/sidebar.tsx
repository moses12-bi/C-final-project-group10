'use client';

import { useState } from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
    Home,
    FolderKanban,
    ListTodo,
    Users,
    BarChart3,
    Calendar,
    Bell,
    Settings,
    ChevronLeft,
    ChevronRight,
    LogOut,
    User,
    UserPlus
} from 'lucide-react';

const navigation = [
    { name: 'Home', href: '/dashboard', icon: Home },
    { name: 'Projects', href: '/projects', icon: FolderKanban },
    { name: 'Task Board', href: '/task-board', icon: ListTodo },
    { name: 'Team', href: '/team', icon: Users },
    { name: 'Calendar', href: '/calendar', icon: Calendar },
    { name: 'Notifications', href: '/notifications', icon: Bell },
    { name: 'Invitations', href: '/invitations', icon: UserPlus },
    { name: 'Settings', href: '/settings/users', icon: Settings },
];

export default function Sidebar() {
    const [collapsed, setCollapsed] = useState(false);
    const pathname = usePathname();

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        localStorage.removeItem('permissions');
        window.location.href = '/login';
    };

    // Don't show sidebar on login/register pages
    if (pathname === '/login' || pathname === '/register') {
        return null;
    }

    return (
        <div
            className={`flex flex-col bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 transition-all duration-300 ${collapsed ? 'w-16' : 'w-64'
                }`}
        >
            {/* Header */}
            <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
                {!collapsed && (
                    <h1 className="text-xl font-bold text-gray-900 dark:text-white">
                        ProjectM
                    </h1>
                )}
                <button
                    onClick={() => setCollapsed(!collapsed)}
                    className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
                >
                    {collapsed ? (
                        <ChevronRight className="w-5 h-5" />
                    ) : (
                        <ChevronLeft className="w-5 h-5" />
                    )}
                </button>
            </div>

            {/* Navigation */}
            <nav className="flex-1 p-4 space-y-2 overflow-y-auto">
                {navigation.map((item) => {
                    const Icon = item.icon;
                    const isActive = pathname === item.href || pathname.startsWith(item.href + '/');

                    return (
                        <Link
                            key={item.name}
                            href={item.href}
                            className={`flex items-center gap-3 px-3 py-2 rounded-lg transition-colors ${isActive
                                ? 'bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400'
                                : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
                                }`}
                            title={collapsed ? item.name : ''}
                        >
                            <Icon className="w-5 h-5 flex-shrink-0" />
                            {!collapsed && <span className="font-medium">{item.name}</span>}
                        </Link>
                    );
                })}
            </nav>

            {/* User Section */}
            <div className="p-4 border-t border-gray-200 dark:border-gray-700 space-y-2">
                <Link
                    href="/profile"
                    className="flex items-center gap-3 px-3 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                    title={collapsed ? 'Profile' : ''}
                >
                    <User className="w-5 h-5 flex-shrink-0" />
                    {!collapsed && <span className="font-medium">Profile</span>}
                </Link>

                <button
                    onClick={handleLogout}
                    className="w-full flex items-center gap-3 px-3 py-2 rounded-lg text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
                    title={collapsed ? 'Logout' : ''}
                >
                    <LogOut className="w-5 h-5 flex-shrink-0" />
                    {!collapsed && <span className="font-medium">Logout</span>}
                </button>
            </div>
        </div>
    );
}
