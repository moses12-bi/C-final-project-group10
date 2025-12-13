'use client';

import { Suspense, useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { LayoutList, KanbanSquare, Plus, Search, Filter } from 'lucide-react';
import api from '@/services/api';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

type ViewType = 'list' | 'board';

type KanbanTask = {
    id: number;
    title: string;
    description: string;
    priority: string;
    status: string;
    estimatedHours: number;
    deadline: string;
    assignees: { userId: string; fullName: string }[];
    project?: { name: string; id: number };
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

function TaskBoardContent() {
    const searchParams = useSearchParams();
    const viewParam = searchParams.get('view') as ViewType | null;
    const projectIdParam = searchParams.get('projectId');

    const [view, setView] = useState<ViewType>(viewParam || 'list');
    const [tasks, setTasks] = useState<any[]>([]);
    const [board, setBoard] = useState<KanbanBoard | null>(null);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');
    const [draggedTask, setDraggedTask] = useState<KanbanTask | null>(null);
    const [selectedProjectId, setSelectedProjectId] = useState<string | null>(projectIdParam);

    useEffect(() => {
        if (view === 'list') {
            loadTasks();
        } else if (view === 'board' && selectedProjectId) {
            loadBoard();
        } else {
            setLoading(false);
        }
    }, [view, selectedProjectId]);

    const loadTasks = async () => {
        setLoading(true);
        try {
            const response = await api.get('/projects');
            const projects = response.data;
            const allTasks: any[] = [];

            for (const project of projects) {
                const tasksResponse = await api.get(`/projects/${project.id}/tasks`);
                const projectTasks = tasksResponse.data.map((task: any) => ({
                    ...task,
                    project: { name: project.name, id: project.id }
                }));
                allTasks.push(...projectTasks);
            }

            setTasks(allTasks);
        } catch (error) {
            console.error('Failed to load tasks', error);
        } finally {
            setLoading(false);
        }
    };

    const loadBoard = async () => {
        if (!selectedProjectId) return;

        setLoading(true);
        try {
            const res = await api.get(`/kanban/project/${selectedProjectId}`);
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
            await api.put(`/kanban/task/${draggedTask.id}/move`, { newStatus });
            await loadBoard();
        } catch (error) {
            console.error('Failed to move task', error);
        } finally {
            setDraggedTask(null);
        }
    };

    const filteredTasks = tasks.filter(task =>
        task.title?.toLowerCase().includes(search.toLowerCase())
    );

    const getStatusColor = (status: string) => {
        const colors: any = {
            'ToDo': 'bg-gray-100 text-gray-800',
            'InProgress': 'bg-blue-100 text-blue-800',
            'Review': 'bg-yellow-100 text-yellow-800',
            'Done': 'bg-green-100 text-green-800',
            'Block': 'bg-red-100 text-red-800'
        };
        return colors[status] || 'bg-gray-100 text-gray-800';
    };

    const getPriorityColor = (priority: string) => {
        const colors: any = {
            'Low': 'text-gray-600',
            'Medium': 'text-blue-600',
            'High': 'text-orange-600',
            'Critical': 'text-red-600'
        };
        return colors[priority] || 'text-gray-600';
    };

    return (
        <div className="h-full flex flex-col">
            {/* Header with View Toggle */}
            <div className="flex items-center justify-between mb-6">
                <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Task Board</h1>

                <div className="flex items-center gap-3">
                    {/* View Toggle Tabs */}
                    <div className="flex gap-1 bg-gray-100 dark:bg-gray-800 p-1 rounded-lg">
                        <button
                            onClick={() => setView('list')}
                            className={`flex items-center gap-2 px-4 py-2 rounded-md transition-colors ${view === 'list'
                                    ? 'bg-white dark:bg-gray-700 shadow-sm text-blue-600 dark:text-blue-400'
                                    : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-200'
                                }`}
                        >
                            <LayoutList className="w-4 h-4" />
                            <span className="font-medium">List</span>
                        </button>
                        <button
                            onClick={() => setView('board')}
                            className={`flex items-center gap-2 px-4 py-2 rounded-md transition-colors ${view === 'board'
                                    ? 'bg-white dark:bg-gray-700 shadow-sm text-blue-600 dark:text-blue-400'
                                    : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-200'
                                }`}
                        >
                            <KanbanSquare className="w-4 h-4" />
                            <span className="font-medium">Board</span>
                        </button>
                    </div>

                    <Link
                        href="/tasks/new"
                        className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        <Plus className="w-5 h-5" />
                        New Task
                    </Link>
                </div>
            </div>

            {/* List View */}
            {view === 'list' && (
                <>
                    {/* Search & Filters */}
                    <div className="flex gap-4 mb-6">
                        <div className="flex-1 relative">
                            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                            <input
                                type="text"
                                placeholder="Search tasks..."
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                className="w-full pl-10 pr-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <button className="flex items-center gap-2 px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700">
                            <Filter className="w-5 h-5" />
                            Filters
                        </button>
                    </div>

                    {/* Tasks Table */}
                    <div className="flex-1 bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
                        {loading ? (
                            <div className="p-8 text-center text-gray-500">Loading tasks...</div>
                        ) : filteredTasks.length === 0 ? (
                            <div className="p-8 text-center text-gray-500">No tasks found</div>
                        ) : (
                            <div className="overflow-x-auto">
                                <table className="w-full">
                                    <thead className="bg-gray-50 dark:bg-gray-700 border-b dark:border-gray-600">
                                        <tr>
                                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Task</th>
                                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Project</th>
                                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
                                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Priority</th>
                                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Deadline</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
                                        {filteredTasks.map((task) => (
                                            <tr key={task.id} className="hover:bg-gray-50 dark:hover:bg-gray-700">
                                                <td className="px-6 py-4">
                                                    <Link href={`/tasks/${task.id}`} className="font-medium text-blue-600 hover:underline">
                                                        {task.title}
                                                    </Link>
                                                </td>
                                                <td className="px-6 py-4 text-sm text-gray-600 dark:text-gray-300">
                                                    {task.project?.name || '-'}
                                                </td>
                                                <td className="px-6 py-4">
                                                    <span className={`px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(task.status)}`}>
                                                        {task.status}
                                                    </span>
                                                </td>
                                                <td className="px-6 py-4">
                                                    <span className={`text-sm font-medium ${getPriorityColor(task.priority)}`}>
                                                        {task.priority}
                                                    </span>
                                                </td>
                                                <td className="px-6 py-4 text-sm text-gray-600 dark:text-gray-300">
                                                    {task.deadline ? new Date(task.deadline).toLocaleDateString() : '-'}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </div>
                </>
            )}

            {/* Board View */}
            {view === 'board' && (
                <>
                    {!selectedProjectId ? (
                        <div className="text-center text-gray-600 dark:text-gray-400 p-8">
                            Please select a project to view the board
                        </div>
                    ) : loading ? (
                        <div className="text-center text-gray-600 dark:text-gray-400 p-8">Loading board...</div>
                    ) : (
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
                    )}
                </>
            )}
        </div>
    );
}

export default function TaskBoardPage() {
    return (
        <Suspense fallback={<div className="p-8">Loading...</div>}>
            <TaskBoardContent />
        </Suspense>
    );
}
