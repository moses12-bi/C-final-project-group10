import api from './api';

export type TaskAssignment = {
    taskId: number;
    userId: string;
    userFullName: string;
    userEmail: string;
    isPrimaryAssignee: boolean;
    assignedAt: string;
};

export type AssignUserRequest = {
    userId: string;
    isPrimaryAssignee?: boolean;
};

export async function listAssignments(taskId: number): Promise<TaskAssignment[]> {
    const res = await api.get(`/tasks/${taskId}/assignments`);
    return res.data;
}

export async function assignUser(taskId: number, payload: AssignUserRequest): Promise<TaskAssignment> {
    const res = await api.post(`/tasks/${taskId}/assignments`, payload);
    return res.data;
}

export async function unassignUser(taskId: number, userId: string): Promise<void> {
    await api.delete(`/tasks/${taskId}/assignments/${userId}`);
}
