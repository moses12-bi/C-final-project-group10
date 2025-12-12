import api from './api';

export type ProjectStatus = 'NotStarted' | 'InProgress' | 'Completed' | 'OnHold' | 'Cancelled';

export type Project = {
  id: number;
  title: string;
  description: string;
  goal: string;
  status: ProjectStatus;
  startDate: string;
  endDate: string;
  createdAt: string;
  managerId: string;
  teamLeadId?: string | null;
};

export type CreateProjectRequest = {
  title: string;
  description: string;
  goal: string;
  status: ProjectStatus;
  startDate: string;
  endDate: string;
  managerId: string;
  teamLeadId?: string | null;
};

export type UpdateProjectRequest = {
  title: string;
  description: string;
  goal: string;
  status: ProjectStatus;
  startDate: string;
  endDate: string;
  teamLeadId?: string | null;
};

export async function listProjects(): Promise<Project[]> {
  const res = await api.get('/projects');
  return res.data;
}

export async function getProject(id: number): Promise<Project> {
  const res = await api.get(`/projects/${id}`);
  return res.data;
}

export async function createProject(payload: CreateProjectRequest): Promise<Project> {
  const res = await api.post('/projects', payload);
  return res.data;
}

export async function updateProject(id: number, payload: UpdateProjectRequest): Promise<void> {
  await api.put(`/projects/${id}`, payload);
}

export async function deleteProject(id: number): Promise<void> {
  await api.delete(`/projects/${id}`);
}
