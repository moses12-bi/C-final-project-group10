'use client';

import { useEffect, useState } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import api from '@/services/api';

type CalendarTask = {
    id: number;
    projectId: number;
    title: string;
    priority: string;
    deadline: string;
    assignees: { userId: string; fullName: string }[];
};

const DAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const MONTHS = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

const priorityColors: Record<string, string> = {
    Low: 'bg-slate-200 text-slate-700',
    Medium: 'bg-blue-200 text-blue-700',
    High: 'bg-orange-200 text-orange-700',
    Critical: 'bg-red-200 text-red-700'
};

export default function CalendarPage() {
    const [currentDate, setCurrentDate] = useState(new Date());
    const [tasks, setTasks] = useState<CalendarTask[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        loadTasks();
    }, [currentDate]);

    const loadTasks = async () => {
        setLoading(true);
        try {
            const year = currentDate.getFullYear();
            const month = currentDate.getMonth();
            const startDate = new Date(year, month, 1).toISOString();
            const endDate = new Date(year, month + 1, 0).toISOString();

            const res = await api.get('/calendar/tasks', {
                params: { startDate, endDate }
            });
            setTasks(res.data);
        } catch (error) {
            console.error('Failed to load tasks', error);
        } finally {
            setLoading(false);
        }
    };

    const getDaysInMonth = () => {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();
        const firstDay = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        const days = [];

        // Empty cells for days before month starts
        for (let i = 0; i < firstDay; i++) {
            days.push(null);
        }

        // Actual days
        for (let i = 1; i <= daysInMonth; i++) {
            days.push(i);
        }

        return days;
    };

    const getTasksForDay = (day: number) => {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();
        const dateStr = new Date(year, month, day).toISOString().split('T')[0];

        return tasks.filter(task => {
            const taskDate = new Date(task.deadline).toISOString().split('T')[0];
            return taskDate === dateStr;
        });
    };

    const previousMonth = () => {
        setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() - 1));
    };

    const nextMonth = () => {
        setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() + 1));
    };

    const days = getDaysInMonth();

    return (
        <div className="min-h-screen bg-slate-50/50 p-4 sm:p-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl sm:text-3xl font-bold text-slate-900">Calendar</h1>
                    <div className="flex items-center gap-2">
                        <Button variant="outline" size="sm" onClick={previousMonth}>
                            <ChevronLeft className="h-4 w-4" />
                        </Button>
                        <span className="text-lg font-medium min-w-[200px] text-center">
                            {MONTHS[currentDate.getMonth()]} {currentDate.getFullYear()}
                        </span>
                        <Button variant="outline" size="sm" onClick={nextMonth}>
                            <ChevronRight className="h-4 w-4" />
                        </Button>
                    </div>
                </div>

                <Card className="p-4">
                    <div className="grid grid-cols-7 gap-2 mb-2">
                        {DAYS.map(day => (
                            <div key={day} className="text-center font-semibold text-sm text-slate-600 p-2">
                                {day}
                            </div>
                        ))}
                    </div>

                    <div className="grid grid-cols-7 gap-2">
                        {days.map((day, index) => {
                            const dayTasks = day ? getTasksForDay(day) : [];
                            const isToday = day &&
                                new Date().getDate() === day &&
                                new Date().getMonth() === currentDate.getMonth() &&
                                new Date().getFullYear() === currentDate.getFullYear();

                            return (
                                <div
                                    key={index}
                                    className={`min-h-[100px] sm:min-h-[120px] p-2 rounded-lg border ${day ? 'bg-white border-slate-200' : 'bg-slate-50 border-transparent'
                                        } ${isToday ? 'ring-2 ring-blue-500' : ''}`}
                                >
                                    {day && (
                                        <>
                                            <div className={`text-sm font-medium mb-1 ${isToday ? 'text-blue-600' : 'text-slate-700'}`}>
                                                {day}
                                            </div>
                                            <div className="space-y-1">
                                                {dayTasks.slice(0, 3).map(task => (
                                                    <div
                                                        key={task.id}
                                                        className={`text-xs p-1 rounded truncate cursor-pointer hover:opacity-80 ${priorityColors[task.priority] || 'bg-slate-100'
                                                            }`}
                                                        title={task.title}
                                                    >
                                                        {task.title}
                                                    </div>
                                                ))}
                                                {dayTasks.length > 3 && (
                                                    <div className="text-xs text-slate-500">
                                                        +{dayTasks.length - 3} more
                                                    </div>
                                                )}
                                            </div>
                                        </>
                                    )}
                                </div>
                            );
                        })}
                    </div>
                </Card>
            </div>
        </div>
    );
}
