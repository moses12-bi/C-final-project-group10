'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { LayoutDashboard, CheckCircle2, Clock, Users, Plus, TrendingUp, Settings } from 'lucide-react';
import api from '@/services/api';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { hasPermission } from '@/lib/permissions';
import { getTaskAnalytics, getTeamPerformanceReport } from '@/services/analytics';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

type DashboardSummary = {
    totalProjects: number;
    activeTasks: number;
    completedTasks: number;
    pendingInvitations: number;
};

export default function DashboardPage() {
    const [summary, setSummary] = useState<DashboardSummary | null>(null);
    const [taskAnalytics, setTaskAnalytics] = useState<any>(null);
    const [teamPerformance, setTeamPerformance] = useState<any[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [analytics, performance] = await Promise.all([
                    getTaskAnalytics(),
                    getTeamPerformanceReport()
                ]);
                setTaskAnalytics(analytics);
                setTeamPerformance(performance);

                // Compute summary from analytics
                const computedSummary = {
                    totalProjects: 0, // Would need projects endpoint
                    activeTasks: analytics?.TotalTasks || 0,
                    completedTasks: analytics?.ByStatus?.find((s: any) => s.Status === 'Done')?.Count || 0,
                    pendingInvitations: 0 // Would need invitations endpoint
                };
                setSummary(computedSummary);
            } catch (error) {
                console.error('Failed to fetch dashboard data', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
    }, []);

    if (isLoading) {
        return (
            <div className="flex h-screen items-center justify-center">
                <div className="h-8 w-8 animate-spin rounded-full border-4 border-blue-600 border-t-transparent"></div>
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
                    <p className="text-gray-600 dark:text-gray-400 mt-1">Welcome back to ProjectM</p>
                </div>
                <div className="flex gap-3">
                    {hasPermission('invites.manage') && (
                        <Link href="/invitations">
                            <Button variant="outline">
                                <Users className="mr-2 h-4 w-4" />
                                Invitations
                            </Button>
                        </Link>
                    )}
                    {hasPermission('users.manage') && (
                        <Link href="/settings/users">
                            <Button variant="outline">
                                <Settings className="mr-2 h-4 w-4" />
                                User Settings
                            </Button>
                        </Link>
                    )}
                    {hasPermission('projects.write') && (
                        <Link href="/projects">
                            <Button>
                                <Plus className="mr-2 h-4 w-4" />
                                New Project
                            </Button>
                        </Link>
                    )}
                </div>
            </div>

            {/* Summary Cards */}
            <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Total Projects</CardTitle>
                        <LayoutDashboard className="h-4 w-4 text-gray-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.totalProjects || 0}</div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Active Tasks</CardTitle>
                        <Clock className="h-4 w-4 text-blue-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.activeTasks || 0}</div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Completed Tasks</CardTitle>
                        <CheckCircle2 className="h-4 w-4 text-green-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{summary?.completedTasks || 0}</div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Completion Rate</CardTitle>
                        <TrendingUp className="h-4 w-4 text-purple-500" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">
                            {taskAnalytics?.CompletionRate?.toFixed(1) || 0}%
                        </div>
                    </CardContent>
                </Card>
            </div>

            {/* Analytics Section */}
            <div>
                <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">Analytics Overview</h2>

                {/* Analytics Cards */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-sm font-medium">Total Tasks</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold">{taskAnalytics?.TotalTasks || 0}</div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle className="text-sm font-medium">Overdue Tasks</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold text-red-600">
                                {taskAnalytics?.OverdueTasks || 0}
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle className="text-sm font-medium">Avg Hours/Task</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold">
                                {taskAnalytics?.AverageEstimatedHours?.toFixed(1) || 0}h
                            </div>
                        </CardContent>
                    </Card>
                </div>

                {/* Charts */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
                    {/* Task Status Distribution */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Tasks by Status</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <ResponsiveContainer width="100%" height={300}>
                                <PieChart>
                                    <Pie
                                        data={taskAnalytics?.ByStatus || []}
                                        dataKey="Count"
                                        nameKey="Status"
                                        cx="50%"
                                        cy="50%"
                                        outerRadius={100}
                                        label
                                    >
                                        {(taskAnalytics?.ByStatus || []).map((entry: any, index: number) => (
                                            <Cell key={entry.Status || `cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                        ))}
                                    </Pie>
                                    <Tooltip />
                                    <Legend />
                                </PieChart>
                            </ResponsiveContainer>
                        </CardContent>
                    </Card>

                    {/* Task Priority Distribution */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Tasks by Priority</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <ResponsiveContainer width="100%" height={300}>
                                <BarChart data={taskAnalytics?.ByPriority || []}>
                                    <CartesianGrid strokeDasharray="3 3" />
                                    <XAxis dataKey="Priority" />
                                    <YAxis />
                                    <Tooltip />
                                    <Legend />
                                    <Bar dataKey="Count" fill="#3b82f6" />
                                </BarChart>
                            </ResponsiveContainer>
                        </CardContent>
                    </Card>
                </div>

                {/* Team Performance */}
                <Card>
                    <CardHeader>
                        <CardTitle>Team Performance</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="overflow-x-auto">
                            <table className="w-full">
                                <thead>
                                    <tr className="border-b border-gray-200 dark:border-gray-700">
                                        <th className="text-left p-3 text-sm font-medium text-gray-600 dark:text-gray-300">Team Member</th>
                                        <th className="text-right p-3 text-sm font-medium text-gray-600 dark:text-gray-300">Total Tasks</th>
                                        <th className="text-right p-3 text-sm font-medium text-gray-600 dark:text-gray-300">Completed</th>
                                        <th className="text-right p-3 text-sm font-medium text-gray-600 dark:text-gray-300">In Progress</th>
                                        <th className="text-right p-3 text-sm font-medium text-gray-600 dark:text-gray-300">Est. Hours</th>
                                        <th className="text-right p-3 text-sm font-medium text-gray-600 dark:text-gray-300">Actual Hours</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {teamPerformance.length === 0 ? (
                                        <tr>
                                            <td colSpan={6} className="p-8 text-center text-gray-500">No team data available</td>
                                        </tr>
                                    ) : (
                                        teamPerformance.map((member: any) => (
                                            <tr key={member.UserId} className="border-b border-gray-100 dark:border-gray-800">
                                                <td className="p-3 text-sm">{member.UserName}</td>
                                                <td className="p-3 text-sm text-right">{member.TotalTasksAssigned}</td>
                                                <td className="p-3 text-sm text-right text-green-600">{member.CompletedTasks}</td>
                                                <td className="p-3 text-sm text-right text-blue-600">{member.InProgressTasks}</td>
                                                <td className="p-3 text-sm text-right">{member.TotalEstimatedHours}h</td>
                                                <td className="p-3 text-sm text-right">{member.TotalActualHours}h</td>
                                            </tr>
                                        ))
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
