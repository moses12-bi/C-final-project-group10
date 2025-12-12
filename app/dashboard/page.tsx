'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { LayoutDashboard, CheckCircle2, Clock, Users, Plus, Settings } from 'lucide-react';
import api from '../../services/api';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { hasPermission } from '@/lib/permissions';

type RecentProject = {
    id: number;
    title: string;
    description?: string;
    status?: string;
};

type DashboardSummary = {
    totalProjects: number;
    activeTasks: number;
    completedTasks: number;
    pendingInvitations: number;
    recentProjects: RecentProject[];
};

export default function DashboardPage() {
    const [summary, setSummary] = useState<DashboardSummary | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchSummary = async () => {
            try {
                const response = await api.get('/dashboard/summary');
                setSummary(response.data);
            } catch (error) {
                console.error('Failed to fetch dashboard summary', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchSummary();
    }, []);

    if (isLoading) {
        return (
            <div className="flex h-screen items-center justify-center">
                <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-900 border-t-transparent"></div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50/50 p-8">
            <div className="mb-8 flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight text-slate-900">Dashboard</h1>
                    <p className="text-slate-500">Welcome back to ProjectM</p>
                </div>
                <div className="flex gap-3">
                    {hasPermission('invites.manage') && (
                        <Link href="/invitations">
                            <Button variant="outline">
                                <Users className="mr-2 h-4 w-4" />
                                Manage Invites
                            </Button>
                        </Link>
                    )}
                    {hasPermission('users.manage') && (
                        <Link href="/settings/users">
                            <Button variant="outline">
                                <Settings className="mr-2 h-4 w-4" />
                                Settings
                            </Button>
                        </Link>
                    )}
                    {hasPermission('projects.write') ? (
                        <Link href="/projects">
                            <Button>
                                <Plus className="mr-2 h-4 w-4" />
                                New Project
                            </Button>
                        </Link>
                    ) : (
                        <Button disabled>
                            <Plus className="mr-2 h-4 w-4" />
                            New Project
                        </Button>
                    )}
                </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Total Projects</CardTitle>
                        <LayoutDashboard className="h-4 w-4 text-slate-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.totalProjects || 0}</div>
                        <p className="text-xs text-slate-500">Projects you are part of</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Active Tasks</CardTitle>
                        <Clock className="h-4 w-4 text-slate-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.activeTasks || 0}</div>
                        <p className="text-xs text-slate-500">Tasks assigned to you</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Completed Tasks</CardTitle>
                        <CheckCircle2 className="h-4 w-4 text-slate-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.completedTasks || 0}</div>
                        <p className="text-xs text-slate-500">Total tasks finished</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Pending Invites</CardTitle>
                        <Users className="h-4 w-4 text-slate-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.pendingInvitations || 0}</div>
                        <p className="text-xs text-slate-500">Invitations awaiting acceptance</p>
                    </CardContent>
                </Card>
            </div>

            <div className="mt-8 grid gap-4 md:grid-cols-2 lg:grid-cols-7">
                <Card className="col-span-4">
                    <CardHeader>
                        <CardTitle>Recent Projects</CardTitle>
                    </CardHeader>
                    <CardContent>
                        {summary?.recentProjects?.length === 0 ? (
                            <p className="text-sm text-slate-500">No projects found.</p>
                        ) : (
                            <div className="space-y-4">
                                {summary?.recentProjects.map((project) => (
                                    <div key={project.id} className="flex items-center justify-between border-b border-slate-100 pb-4 last:border-0 last:pb-0">
                                        <div>
                                            <p className="font-medium text-slate-900">{project.title}</p>
                                            <p className="text-sm text-slate-500 truncate max-w-[300px]">{project.description}</p>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${project.status === 'Completed' ? 'bg-green-100 text-green-800' :
                                                project.status === 'InProgress' ? 'bg-blue-100 text-blue-800' :
                                                    'bg-slate-100 text-slate-800'
                                                }`}>
                                                {project.status || 'Active'}
                                            </span>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </CardContent>
                </Card>

                {/* Placeholder for chart or activity feed */}
                <Card className="col-span-3">
                    <CardHeader>
                        <CardTitle>Quick Actions</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-2">
                        <Button variant="secondary" className="w-full justify-start">Create New Task</Button>
                        <Button variant="secondary" className="w-full justify-start">View My Calendar</Button>
                        <Button variant="secondary" className="w-full justify-start">Generated Reports</Button>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
