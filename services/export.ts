import api from './api';

export async function exportProjectPDF(projectId: number) {
    const res = await api.get(`/export/project/${projectId}/pdf`, {
        responseType: 'blob'
    });
    return res.data;
}

export async function exportProjectExcel(projectId: number) {
    const res = await api.get(`/export/project/${projectId}/excel`, {
        responseType: 'blob'
    });
    return res.data;
}

export async function exportTasksExcel(projectId?: number) {
    const res = await api.get('/export/tasks/excel', {
        params: { projectId },
        responseType: 'blob'
    });
    return res.data;
}

export function downloadFile(blob: Blob, fileName: string) {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
}
