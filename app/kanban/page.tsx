'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import api from '@/services/api';

type KanbanTask = {
    id: number;
    title: string;
    description: string;
    priority: string;
    status: string;
    estimatedHours: number;
    deadline: string;
    assignees: { userId: string; fullName: string }[];
};

type KanbanBoard = {
    ToDo: KanbanTask[];
    InProgress: KanbanTask[];
    Review: KanbanTask[];
    Done: KanbanTask[];
    Block: KanbanTask[];
};

const statusColumns = [
    { key: 'ToDo', label: 'To Do', color: 'bg-slate-100' },
    { key: 'InProgress', label: 'In Progress', color: 'bg-blue-100' },
    { key: 'Review', label: 'Review', color: 'bg-yellow-100' },
    { key: 'Done', label: 'Done', color: 'bg-green-100' },
    { key: 'Block', label: 'Blocked', color: 'bg-red-100' }
];

const priorityColors = {
    Low: 'bg-slate-200',
    Medium: 'bg-blue-200',
    High: 'bg-orange-200',
    Critical: 'bg-red-200'
};

export default function KanbanPage() {
    const searchParams = useSearchParams();
    const projectId = searchParams.get('projectId');

    const [board, setBoard] = useState<KanbanBoard | null>(null);
    const [loading, setLoading] = useState(true);
    const [draggedTask, setDraggedTask] = useState<KanbanTask | null>(null);

    useEffect(() => {
        if (projectId) {
            loadBoard();
        }
    }, [projectId]);

    const loadBoard = async () => {
        if (!projectId) return;

        setLoading(true);
        try {
            const res = await api.get(`/kanban/project/${projectId}`);
            setBoard(res.data);
        } catch (error) {
            console.error('Failed to load Kanban board', error);
        } finally {
            setLoading(false);
        }
    };

    const handleDragStart = (task: KanbanTask) => {
        setDraggedTask(task);
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
    };

    const handleDrop = async (newStatus: string) => {
        if (!draggedTask) return;

        try {
            await api.put(`/kanban/task/${draggedTask.id}/move`, {
                newStatus
            });
            await loadBoard();
        } catch (error) {
            console.error('Failed to move task', error);
        } finally {
            setDraggedTask(null);
        }
    };

    if (!projectId) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">
                    Please select a project to view the Kanban board
                </div>
            </div>
        );
    }

    if (loading) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">Loading board...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50/50 p-8">
            <div className="max-w-7xl mx-auto">
                <h1 className="text-3xl font-bold text-slate-900 mb-8">Kanban Board</h1>

                <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-5 gap-4">
                    {statusColumns.map((column) => (
                        <div
                            key={column.key}
                            className="flex flex-col"
                            onDragOver={handleDragOver}
                            onDrop={() => handleDrop(column.key)}
                        >
                            <Card className={`${column.color} min-h-[500px]`}>
                                <CardHeader>
                                    <CardTitle className="text-sm font-semibold">
                                        {column.label}
                                        <span className="ml-2 text-xs text-slate-600">
                                            ({board?.[column.key as keyof KanbanBoard]?.length || 0})
                                        </span>
                                    </CardTitle>
                                </CardHeader>
                                <CardContent className="space-y-3">
                                    {board?.[column.key as keyof KanbanBoard]?.map((task) => (
                                        <div
                                            key={task.id}
                                            draggable
                                            onDragStart={() => handleDragStart(task)}
                                            className="bg-white p-3 rounded-lg shadow-sm border border-slate-200 cursor-move hover:shadow-md transition-shadow"
                                        >
                                            <div className="flex items-start justify-between gap-2 mb-2">
                                                <h4 className="font-medium text-sm text-slate-900 line-clamp-2">
                                                    {task.title}
                                                </h4>
                                                <span className={`text-xs px-2 py-1 rounded ${priorityColors[task.priority as keyof typeof priorityColors]}`}>
                                                    {task.priority}
                                                </span>
                                            </div>

                                            {task.description && (
                                                <p className="text-xs text-slate-600 line-clamp-2 mb-2">
                                                    {task.description}
                                                </p>
                                            )}

                                            <div className="flex items-center justify-between text-xs text-slate-500">
                                                <span>{task.estimatedHours}h</span>
                                                {task.assignees.length > 0 && (
                                                    <div className="flex -space-x-2">
                                                        {task.assignees.slice(0, 3).map((assignee) => (
                                                            <div
                                                                key={assignee.userId}
                                                                className="w-6 h-6 rounded-full bg-blue-500 text-white flex items-center justify-center text-xs border-2 border-white"
                                                                title={assignee.fullName}
                                                            >
                                                                {assignee.fullName.charAt(0)}
                                                            </div>
                                                        ))}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    ))}
                                </CardContent>
                            </Card>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
