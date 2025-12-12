import api from './api';

export type Notification = {
    id: number;
    type: string;
    title: string;
    message: string;
    isRead: boolean;
    createdAt: string;
    relatedEntityId?: number | null;
    relatedEntityType?: string | null;
};

export async function listNotifications(unreadOnly: boolean = false): Promise<Notification[]> {
    const res = await api.get('/notifications', {
        params: { unreadOnly }
    });
    return res.data;
}

export async function markAsRead(id: number): Promise<void> {
    await api.put(`/notifications/${id}/read`);
}

export async function markAllAsRead(): Promise<void> {
    await api.put('/notifications/read-all');
}

export async function deleteNotification(id: number): Promise<void> {
    await api.delete(`/notifications/${id}`);
}
