export type StoredUser = {
  id: string;
  email?: string;
  fullName?: string;
  role?: string;
  department?: string;
};

export function getStoredUser(): StoredUser | null {
  if (typeof window === 'undefined') return null;
  try {
    const raw = localStorage.getItem('user');
    if (!raw) return null;
    return JSON.parse(raw) as StoredUser;
  } catch {
    return null;
  }
}
