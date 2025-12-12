import api from './api';

export type TaskStatus = 'ToDo' | 'InProgress' | 'Review' | 'Done' | 'Block';
export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export type ProjectTask = {
  id: number;
  title: string;
  description: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: string;
  deadline: string;
  estimatedHours: number;
  actualHours?: number | null;
  projectId: number;
};

export type CreateTaskRequest = {
  title: string;
  description: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: string;
  deadline: string;
  estimatedHours: number;
};

export type UpdateTaskRequest = {
  title: string;
  description: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: string;
  deadline: string;
  estimatedHours: number;
  actualHours?: number | null;
};

export async function listTasks(projectId: number): Promise<ProjectTask[]> {
  const res = await api.get(`/projects/${projectId}/tasks`);
  return res.data;
}

export async function createTask(projectId: number, payload: CreateTaskRequest): Promise<ProjectTask> {
  const res = await api.post(`/projects/${projectId}/tasks`, payload);
  return res.data;
}

export async function updateTask(projectId: number, taskId: number, payload: UpdateTaskRequest): Promise<void> {
  await api.put(`/projects/${projectId}/tasks/${taskId}`, payload);
}

export async function deleteTask(projectId: number, taskId: number): Promise<void> {
  await api.delete(`/projects/${projectId}/tasks/${taskId}`);
}
