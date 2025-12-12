import api from './api';

export type TeamMember = {
    userId: string;
    fullName: string;
    email: string;
    role: string;
    department: string;
};

export type AddTeamMemberRequest = {
    userId: string;
};

export type UpdateTeamLeadRequest = {
    teamLeadId: string;
};

export async function listTeamMembers(projectId: number): Promise<TeamMember[]> {
    const res = await api.get(`/projects/${projectId}/team`);
    return res.data;
}

export async function addTeamMember(projectId: number, payload: AddTeamMemberRequest): Promise<TeamMember> {
    const res = await api.post(`/projects/${projectId}/team`, payload);
    return res.data;
}

export async function removeTeamMember(projectId: number, userId: string): Promise<void> {
    await api.delete(`/projects/${projectId}/team/${userId}`);
}

export async function updateTeamLead(projectId: number, payload: UpdateTeamLeadRequest): Promise<void> {
    await api.put(`/projects/${projectId}/team/lead`, payload);
}
