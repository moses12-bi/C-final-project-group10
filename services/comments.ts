import api from './api';

export interface Comment {
    id: number;
    taskId: number;
    content: string;
    userId: string;
    userFullName: string;
    createdAt: string;
}

export async function listComments(taskId: number): Promise<Comment[]> {
    const res = await api.get(`/tasks/${taskId}/comments`);
    return res.data;
}

export async function createComment(taskId: number, dto: { content: string }): Promise<Comment> {
    const res = await api.post(`/tasks/${taskId}/comments`, dto);
    return res.data;
}

export async function updateComment(taskId: number, commentId: number, content: string) {
    const res = await api.put(`/tasks/${taskId}/comments/${commentId}`, { content });
    return res.data;
}

export async function deleteComment(taskId: number, commentId: number) {
    await api.delete(`/tasks/${taskId}/comments/${commentId}`);
}
