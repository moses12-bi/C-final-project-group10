'use client';

import { useEffect, useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { getTaskAnalytics, getTeamPerformanceReport } from '@/services/analytics';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

export default function AnalyticsPage() {
    const [taskAnalytics, setTaskAnalytics] = useState<any>(null);
    const [teamPerformance, setTeamPerformance] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadAnalytics();
    }, []);

    const loadAnalytics = async () => {
        setLoading(true);
        try {
            const [analytics, performance] = await Promise.all([
                getTaskAnalytics(),
                getTeamPerformanceReport()
            ]);
            setTaskAnalytics(analytics);
            setTeamPerformance(performance);
        } catch (error) {
            console.error('Failed to load analytics', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">Loading analytics...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50/50 p-8">
            <div className="max-w-7xl mx-auto">
                <h1 className="text-3xl font-bold text-slate-900 mb-8">Analytics Dashboard</h1>

                {/* Summary Cards */}
                <div className="grid grid-cols-4 gap-6 mb-8">
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
                            <CardTitle className="text-sm font-medium">Completion Rate</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold">
                                {taskAnalytics?.CompletionRate?.toFixed(1) || 0}%
                            </div>
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
                <div className="grid grid-cols-2 gap-6 mb-8">
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
                                            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
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
                                    <tr className="border-b border-slate-200">
                                        <th className="text-left p-3 text-sm font-medium text-slate-600">Team Member</th>
                                        <th className="text-right p-3 text-sm font-medium text-slate-600">Total Tasks</th>
                                        <th className="text-right p-3 text-sm font-medium text-slate-600">Completed</th>
                                        <th className="text-right p-3 text-sm font-medium text-slate-600">In Progress</th>
                                        <th className="text-right p-3 text-sm font-medium text-slate-600">Est. Hours</th>
                                        <th className="text-right p-3 text-sm font-medium text-slate-600">Actual Hours</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {teamPerformance.map((member: any) => (
                                        <tr key={member.UserId} className="border-b border-slate-100">
                                            <td className="p-3 text-sm">{member.UserName}</td>
                                            <td className="p-3 text-sm text-right">{member.TotalTasksAssigned}</td>
                                            <td className="p-3 text-sm text-right text-green-600">{member.CompletedTasks}</td>
                                            <td className="p-3 text-sm text-right text-blue-600">{member.InProgressTasks}</td>
                                            <td className="p-3 text-sm text-right">{member.TotalEstimatedHours}h</td>
                                            <td className="p-3 text-sm text-right">{member.TotalActualHours}h</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
