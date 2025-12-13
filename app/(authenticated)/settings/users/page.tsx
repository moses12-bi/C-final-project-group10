'use client';

import { useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import api from '@/services/api';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { hasPermission } from '@/lib/permissions';

type Permission = {
  id: number;
  code: string;
  description: string;
};

type AdminUserDto = {
  id: string;
  email: string;
  fullName: string;
  role: string;
  department: string;
  isActive: boolean;
  createdAt: string;
  permissions: Record<string, boolean>;
};

export default function UserPermissionsSettingsPage() {
  const [users, setUsers] = useState<AdminUserDto[]>([]);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [savingUserId, setSavingUserId] = useState<string | null>(null);

  const canManage = hasPermission('users.manage');

  useEffect(() => {
    if (!canManage) {
      setError('Not authorized');
      setIsLoading(false);
      return;
    }

    const load = async () => {
      setIsLoading(true);
      setError('');
      try {
        const [uRes, pRes] = await Promise.all([
          api.get('/users'),
          api.get('/permissions')
        ]);
        setUsers(uRes.data);
        setPermissions(pRes.data);
      } catch {
        setError('Failed to load users/permissions');
      } finally {
        setIsLoading(false);
      }
    };

    load();
  }, [canManage]);

  const permissionCodes = useMemo(() => permissions.map(p => p.code), [permissions]);

  const updatePermission = async (userId: string, code: string, value: boolean) => {
    const user = users.find(u => u.id === userId);
    if (!user) return;

    const nextPerms = {
      ...(user.permissions || {})
    };

    // Ensure all known permissions exist in payload so backend can store a complete set
    for (const c of permissionCodes) {
      if (typeof nextPerms[c] !== 'boolean') nextPerms[c] = false;
    }

    nextPerms[code] = value;

    // optimistic UI
    setUsers(prev => prev.map(u => (u.id === userId ? { ...u, permissions: nextPerms } : u)));
    setSavingUserId(userId);

    try {
      await api.put(`/users/${userId}/permissions`, nextPerms);
    } catch {
      setError('Failed to save permissions');
    } finally {
      setSavingUserId(null);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-slate-50/50 p-8">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-900 border-t-transparent" />
      </div>
    );
  }

  if (!canManage) {
    return (
      <div className="min-h-screen bg-slate-50/50 p-8">
        <Card>
          <CardHeader>
            <CardTitle>Settings</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-slate-600">You do not have permission to manage users.</p>
            <div className="mt-4">
              <Link href="/dashboard">
                <Button variant="outline">Back to Dashboard</Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-slate-900">Settings</h1>
          <p className="text-slate-500">Manage user permissions</p>
        </div>
        <Link href="/dashboard">
          <Button variant="outline">Back</Button>
        </Link>
      </div>

      {error && (
        <div className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-600 ring-1 ring-red-200">
          {error}
        </div>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Permissions Matrix</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-auto">
            <table className="min-w-[900px] border-collapse text-sm">
              <thead>
                <tr className="border-b border-slate-200">
                  <th className="sticky left-0 z-10 bg-white p-3 text-left font-semibold text-slate-900">Permission</th>
                  {users.map(u => (
                    <th key={u.id} className="p-3 text-left font-semibold text-slate-900">
                      <div className="flex flex-col">
                        <span className="truncate max-w-[180px]">{u.fullName || u.email}</span>
                        <span className="text-xs font-normal text-slate-500">{u.email}</span>
                      </div>
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {permissions.map(p => (
                  <tr key={p.code} className="border-b border-slate-100">
                    <td className="sticky left-0 z-10 bg-white p-3">
                      <div className="flex flex-col">
                        <span className="font-medium text-slate-900">{p.code}</span>
                        <span className="text-xs text-slate-500">{p.description}</span>
                      </div>
                    </td>
                    {users.map(u => {
                      const checked = u.permissions?.[p.code] === true;
                      const disabled = savingUserId === u.id;
                      return (
                        <td key={`${u.id}:${p.code}`} className="p-3">
                          <input
                            type="checkbox"
                            className="h-4 w-4"
                            checked={checked}
                            disabled={disabled}
                            onChange={(e) => updatePermission(u.id, p.code, e.target.checked)}
                          />
                        </td>
                      );
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
