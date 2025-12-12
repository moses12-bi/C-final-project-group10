import api from './api';

export async function updateComment(taskId: number, commentId: number, content: string) {
    const res = await api.put(`/tasks/${taskId}/comments/${commentId}`, { content });
    return res.data;
}

export async function deleteComment(taskId: number, commentId: number) {
    await api.delete(`/tasks/${taskId}/comments/${commentId}`);
}
