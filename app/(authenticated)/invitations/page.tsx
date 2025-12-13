'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { ArrowLeft, Send } from 'lucide-react';
import api from '@/services/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { hasPermission } from '@/lib/permissions';

type Invitation = {
    invitationId: string;
    email: string;
    role: string;
    department: string;
    status: string;
    expiresAt: string;
    createdAt: string;
    invitedByUserName: string;
};

export default function InvitationsPage() {
    const canManageInvites = hasPermission('invites.manage');

    const [email, setEmail] = useState('');
    const [role, setRole] = useState('User');
    const [department, setDepartment] = useState('Engineering');
    const [isLoading, setIsLoading] = useState(false);
    const [successMessage, setSuccessMessage] = useState('');
    const [error, setError] = useState('');
    const [pendingInvites, setPendingInvites] = useState<Invitation[]>([]);

    const loadPending = async () => {
        if (!canManageInvites) return;
        try {
            const res = await api.get('/invitation');
            setPendingInvites(res.data);
        } catch {
            // ignore
        }
    };

    useEffect(() => {
        loadPending();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const handleInvite = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setSuccessMessage('');
        setError('');

        try {
            await api.post('/invitation', {
                email,
                role,
                department,
                permissions: {
                    'projects.read': true,
                    'tasks.read': true,
                    'tasks.write': true,
                    'analytics.read': true,
                    'calendar.read': true,
                    'notifications.read': true,
                    'files.read': true
                }
            });
            setSuccessMessage(`Invitation sent to ${email}`);
            setEmail('');
            await loadPending();
        } catch (err: unknown) {
            console.error(err);
            setError('Failed to send invitation. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50/50 p-8">
            {!canManageInvites && (
                <div className="mb-4 rounded-md bg-yellow-50 p-3 text-sm text-yellow-800 ring-1 ring-yellow-200">
                    You do not have permission to manage invitations.
                </div>
            )}
            <div className="mb-8">
                <Link href="/dashboard" className="mb-4 inline-flex items-center text-sm text-slate-500 hover:text-slate-900">
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Back to Dashboard
                </Link>
                <h1 className="text-3xl font-bold tracking-tight text-slate-900">Manage Invitations</h1>
                <p className="text-slate-500">Invite new users to the platform</p>
            </div>

            <div className="grid gap-8 md:grid-cols-2">
                <Card>
                    <CardHeader>
                        <CardTitle>Send New Invitation</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <form onSubmit={handleInvite} className="space-y-4">
                            <fieldset disabled={!canManageInvites || isLoading} className="space-y-4">
                                {successMessage && (
                                    <div className="rounded-md bg-green-50 p-3 text-sm text-green-600 ring-1 ring-green-200">
                                        {successMessage}
                                    </div>
                                )}
                                {error && (
                                    <div className="rounded-md bg-red-50 p-3 text-sm text-red-500 ring-1 ring-red-200">
                                        {error}
                                    </div>
                                )}

                                <div className="space-y-2">
                                    <label className="text-sm font-medium">Email Address</label>
                                    <Input
                                        type="email"
                                        required
                                        placeholder="colleague@company.com"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                    />
                                </div>

                                <div className="grid grid-cols-2 gap-4">
                                    <div className="space-y-2">
                                        <label className="text-sm font-medium">Role</label>
                                        <select
                                            className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-slate-950"
                                            value={role}
                                            onChange={(e) => setRole(e.target.value)}
                                        >
                                            <option value="User">User</option>
                                            <option value="Manager">Manager</option>
                                            <option value="Admin">Admin</option>
                                        </select>
                                    </div>
                                    <div className="space-y-2">
                                        <label className="text-sm font-medium">Department</label>
                                        <select
                                            className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-slate-950"
                                            value={department}
                                            onChange={(e) => setDepartment(e.target.value)}
                                        >
                                            <option value="Engineering">Engineering</option>
                                            <option value="Design">Design</option>
                                            <option value="Product">Product</option>
                                            <option value="Marketing">Marketing</option>
                                        </select>
                                    </div>
                                </div>

                                <Button type="submit" className="w-full" isLoading={isLoading}>
                                    <Send className="mr-2 h-4 w-4" />
                                    Send Invitation
                                </Button>
                            </fieldset>
                        </form>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle>Pending Invitations</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-4">
                            {pendingInvites.length === 0 ? (
                                <div className="rounded-lg border border-dashed border-slate-200 p-8 text-center text-slate-500">
                                    No pending invitations.
                                </div>
                            ) : (
                                <div className="space-y-3">
                                    {pendingInvites.map((inv) => (
                                        <div key={inv.invitationId} className="flex items-start justify-between rounded-md border border-slate-200 bg-white p-3">
                                            <div>
                                                <div className="font-medium text-slate-900">{inv.email}</div>
                                                <div className="text-xs text-slate-500">{inv.department} • {inv.role} • invited by {inv.invitedByUserName}</div>
                                            </div>
                                            <div className="text-xs text-slate-500">{new Date(inv.createdAt).toLocaleString()}</div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
