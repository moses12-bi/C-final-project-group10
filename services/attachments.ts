import api from './api';

export type Attachment = {
    id: number;
    taskId: number;
    fileName: string;
    fileSize: number;
    contentType: string;
    uploadedAt: string;
    uploadedBy: string;
};

export async function listAttachments(taskId: number): Promise<Attachment[]> {
    const res = await api.get(`/tasks/${taskId}/attachments`);
    return res.data;
}

export async function uploadAttachment(taskId: number, file: File): Promise<Attachment> {
    const formData = new FormData();
    formData.append('file', file);

    const res = await api.post(`/tasks/${taskId}/attachments`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
    return res.data;
}

export async function downloadAttachment(taskId: number, attachmentId: number): Promise<Blob> {
    const res = await api.get(`/tasks/${taskId}/attachments/${attachmentId}/download`, {
        responseType: 'blob',
    });
    return res.data;
}

export async function deleteAttachment(taskId: number, attachmentId: number): Promise<void> {
    await api.delete(`/tasks/${taskId}/attachments/${attachmentId}`);
}
