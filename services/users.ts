import api from './api';

export type User = {
    id: string;
    fullName: string;
    email: string;
    role: string;
    department: string | null;
};

export async function listAllUsers(): Promise<User[]> {
    const res = await api.get('/users');
    return res.data;
}

export async function getUser(id: string): Promise<User> {
    const res = await api.get(`/users/${id}`);
    return res.data;
}
