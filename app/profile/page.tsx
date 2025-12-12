'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { getStoredUser } from '@/lib/session';
import api from '@/services/api';

type UserProfile = {
    id: string;
    fullName: string;
    email: string;
    role: string;
    department: string | null;
    createdAt: string;
};

export default function ProfilePage() {
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [editing, setEditing] = useState(false);
    const [fullName, setFullName] = useState('');
    const [department, setDepartment] = useState('');
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        loadProfile();
    }, []);

    const loadProfile = async () => {
        setLoading(true);
        try {
            const res = await api.get('/users/me');
            setProfile(res.data);
            setFullName(res.data.fullName);
            setDepartment(res.data.department || '');
        } catch (error) {
            console.error('Failed to load profile', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            await api.put('/users/me', {
                fullName,
                department: department || null
            });
            await loadProfile();
            setEditing(false);
        } catch (error) {
            console.error('Failed to update profile', error);
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">Loading profile...</div>
            </div>
        );
    }

    if (!profile) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">Profile not found</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50/50 p-4 sm:p-8">
            <div className="max-w-3xl mx-auto">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl sm:text-3xl font-bold text-slate-900">My Profile</h1>
                    <Link href="/dashboard">
                        <Button variant="outline">Back to Dashboard</Button>
                    </Link>
                </div>

                <Card>
                    <CardHeader>
                        <CardTitle>Personal Information</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                        {editing ? (
                            <>
                                <div>
                                    <label className="block text-sm font-medium mb-1">Full Name</label>
                                    <Input
                                        value={fullName}
                                        onChange={(e) => setFullName(e.target.value)}
                                        placeholder="Enter your full name"
                                    />
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">Email</label>
                                    <Input value={profile.email} disabled className="bg-slate-100" />
                                    <p className="text-xs text-slate-500 mt-1">Email cannot be changed</p>
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">Department</label>
                                    <Input
                                        value={department}
                                        onChange={(e) => setDepartment(e.target.value)}
                                        placeholder="Enter your department"
                                    />
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">Role</label>
                                    <Input value={profile.role} disabled className="bg-slate-100" />
                                    <p className="text-xs text-slate-500 mt-1">Contact admin to change role</p>
                                </div>

                                <div className="flex gap-2 pt-4">
                                    <Button onClick={handleSave} isLoading={saving}>
                                        Save Changes
                                    </Button>
                                    <Button variant="outline" onClick={() => setEditing(false)}>
                                        Cancel
                                    </Button>
                                </div>
                            </>
                        ) : (
                            <>
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-slate-600">Full Name</label>
                                        <p className="text-slate-900 mt-1">{profile.fullName}</p>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-slate-600">Email</label>
                                        <p className="text-slate-900 mt-1">{profile.email}</p>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-slate-600">Department</label>
                                        <p className="text-slate-900 mt-1">{profile.department || 'Not set'}</p>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-slate-600">Role</label>
                                        <p className="text-slate-900 mt-1">
                                            <span className="px-2 py-1 bg-blue-100 text-blue-700 rounded text-sm">
                                                {profile.role}
                                            </span>
                                        </p>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-slate-600">Member Since</label>
                                        <p className="text-slate-900 mt-1">
                                            {new Date(profile.createdAt).toLocaleDateString()}
                                        </p>
                                    </div>
                                </div>

                                <div className="pt-4">
                                    <Button onClick={() => setEditing(true)}>
                                        Edit Profile
                                    </Button>
                                </div>
                            </>
                        )}
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
