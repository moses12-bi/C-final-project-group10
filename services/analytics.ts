import api from './api';

export async function globalSearch(query: string, type?: 'projects' | 'tasks' | 'users') {
    const res = await api.get('/search', {
        params: { query, type }
    });
    return res.data;
}

export async function getProjectStatusReport(projectId: number) {
    const res = await api.get(`/reports/project-status/${projectId}`);
    return res.data;
}

export async function getTeamPerformanceReport(startDate?: string, endDate?: string) {
    const res = await api.get('/reports/team-performance', {
        params: { startDate, endDate }
    });
    return res.data;
}

export async function getTaskAnalytics(projectId?: number) {
    const res = await api.get('/reports/task-analytics', {
        params: { projectId }
    });
    return res.data;
}
