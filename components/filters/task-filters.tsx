'use client';

import { FiltersBar, FilterProps } from '@/components/ui/filters-bar';
import { useState, useEffect } from 'react';
import { listTasks, type ProjectTask } from '@/services/tasks';
import { listAllUsers, type User } from '@/services/users';

interface TaskFiltersProps {
    projectId?: number;
    onFilteredTasks: (tasks: ProjectTask[]) => void;
}

export function TaskFilters({ projectId, onFilteredTasks }: TaskFiltersProps) {
    const [users, setUsers] = useState<User[]>([]);
    const [status, setStatus] = useState('all');
    const [priority, setPriority] = useState('all');
    const [assignedTo, setAssignedTo] = useState('all');

    useEffect(() => {
        loadUsers();
    }, []);

    useEffect(() => {
        filterTasks();
    }, [status, priority, assignedTo, projectId]);

    const loadUsers = async () => {
        try {
            const data = await listAllUsers();
            setUsers(data);
        } catch (error) {
            console.error('Failed to load users', error);
        }
    };

    const filterTasks = async () => {
        try {
            let tasks: ProjectTask[] = [];
            if (projectId) {
                tasks = await listTasks(projectId);
            }

            // Apply filters
            let filtered = tasks;

            if (status !== 'all') {
                filtered = filtered.filter(t => t.status === status);
            }

            if (priority !== 'all') {
                filtered = filtered.filter(t => t.priority === priority);
            }

            if (assignedTo !== 'all') {
                // This would require assignment data - simplified for now
                filtered = filtered;
            }

            onFilteredTasks(filtered);
        } catch (error) {
            console.error('Failed to filter tasks', error);
        }
    };

    const handleReset = () => {
        setStatus('all');
        setPriority('all');
        setAssignedTo('all');
    };

    const filters: FilterProps[] = [
        {
            label: 'Status',
            value: status,
            onChange: setStatus,
            options: [
                { value: 'all', label: 'All Statuses' },
                { value: 'ToDo', label: 'To Do' },
                { value: 'InProgress', label: 'In Progress' },
                { value: 'Review', label: 'Review' },
                { value: 'Done', label: 'Done' },
                { value: 'Block', label: 'Blocked' }
            ]
        },
        {
            label: 'Priority',
            value: priority,
            onChange: setPriority,
            options: [
                { value: 'all', label: 'All Priorities' },
                { value: 'Low', label: 'Low' },
                { value: 'Medium', label: 'Medium' },
                { value: 'High', label: 'High' },
                { value: 'Critical', label: 'Critical' }
            ]
        },
        {
            label: 'Assigned To',
            value: assignedTo,
            onChange: setAssignedTo,
            options: [
                { value: 'all', label: 'All Users' },
                ...users.map(u => ({ value: u.id, label: u.fullName }))
            ]
        }
    ];

    return <FiltersBar filters={filters} onReset={handleReset} />;
}
