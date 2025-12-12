'use client';

import { Suspense, useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { CheckCircle2, UserPlus } from 'lucide-react';
import Link from 'next/link';
import api from '../../services/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

function RegisterForm() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const token = searchParams.get('token');

    const [fullName, setFullName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    type InviteData = { email: string; department: string; role: string };
    const [isValidToken, setIsValidToken] = useState(false);
    const [inviteData, setInviteData] = useState<InviteData | null>(null);

    const isInviteMode = !!token;

    useEffect(() => {
        if (!isInviteMode) return;

        // Validate token on mount
        const checkToken = async () => {
            try {
                const response = await api.get(`/invitation/${token}`);
                setInviteData(response.data);
                setEmail(response.data.email); // Pre-fill email from invite
                setIsValidToken(true);
            } catch {
                setError('Invalid or expired invitation token.');
            }
        };
        checkToken();
    }, [token, isInviteMode]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            let response;

            if (isInviteMode) {
                response = await api.post('/auth/complete-registration', {
                    token,
                    fullName,
                    password
                });
            } else {
                response = await api.post('/auth/register', {
                    email,
                    fullName,
                    password
                });
            }

            if (response.data && response.data.token) {
                localStorage.setItem('token', response.data.token);
                if (response.data.user) {
                    localStorage.setItem('user', JSON.stringify(response.data.user));
                }
                if (response.data.permissions) {
                    localStorage.setItem('permissions', JSON.stringify(response.data.permissions));
                }
                router.push('/dashboard');
            }
        } catch (err: unknown) {
            const maybe = err as { response?: { data?: unknown } };
            setError(typeof maybe.response?.data === 'string' ? (maybe.response?.data as string) : 'Registration failed');
        } finally {
            setIsLoading(false);
        }
    };

    if (isInviteMode && error && !inviteData) {
        return (
            <div className="text-center">
                <h1 className="text-xl font-bold text-red-600">Invitation Error</h1>
                <p className="mt-2 text-slate-500">{error}</p>
                <div className="mt-4">
                    <Link href="/register" className="text-blue-600 hover:underline">Register for a new account instead</Link>
                </div>
            </div>
        );
    }

    if (isInviteMode && !isValidToken) {
        return <div className="text-center">Validating invitation...</div>;
    }

    return (
        <div className="w-full max-w-md space-y-8 rounded-xl bg-white p-8 shadow-lg ring-1 ring-slate-900/5">
            <div className="text-center">
                <h1 className="text-3xl font-bold tracking-tight text-slate-900">
                    {isInviteMode ? 'Complete Setup' : 'Create Account'}
                </h1>
                <p className="mt-2 text-sm text-slate-600">
                    {isInviteMode ? (
                        <>Welcome, <strong>{inviteData.email}</strong>!</>
                    ) : (
                        'Get started with ProjectM'
                    )}
                </p>
                {isInviteMode && (
                    <div className="mt-2 flex items-center justify-center gap-2 text-xs text-slate-500">
                        <span className="inline-flex items-center rounded-full bg-blue-50 px-2 py-1 text-blue-700 ring-1 ring-inset ring-blue-700/10">{inviteData.department}</span>
                        <span className="inline-flex items-center rounded-full bg-indigo-50 px-2 py-1 text-indigo-700 ring-1 ring-inset ring-indigo-700/10">{inviteData.role}</span>
                    </div>
                )}
            </div>

            <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
                {error && (
                    <div className="rounded-md bg-red-50 p-3 text-sm text-red-500 ring-1 ring-red-200">
                        {error}
                    </div>
                )}

                <div className="space-y-4">
                    {!isInviteMode && (
                        <div>
                            <label className="block text-sm font-medium text-slate-700">Email Address</label>
                            <Input
                                type="email"
                                required
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="name@company.com"
                            />
                        </div>
                    )}

                    <div>
                        <label className="block text-sm font-medium text-slate-700">Full Name</label>
                        <Input
                            required
                            value={fullName}
                            onChange={(e) => setFullName(e.target.value)}
                            placeholder="John Doe"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700">Password</label>
                        <Input
                            type="password"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="••••••••"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700">Confirm Password</label>
                        <Input
                            type="password"
                            required
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            placeholder="••••••••"
                        />
                    </div>
                </div>

                <Button type="submit" className="w-full" isLoading={isLoading}>
                    {isInviteMode ? <CheckCircle2 className="mr-2 h-4 w-4" /> : <UserPlus className="mr-2 h-4 w-4" />}
                    {isInviteMode ? 'Complete Registration' : 'Create Account'}
                </Button>

                {!isInviteMode && (
                    <div className="text-center text-sm">
                        <span className="text-slate-500">Already have an account? </span>
                        <Link href="/login" className="font-semibold text-slate-900 hover:text-slate-700">
                            Sign in
                        </Link>
                    </div>
                )}
            </form>
        </div>
    );
}

export default function RegisterPage() {
    return (
        <div className="flex min-h-screen flex-col items-center justify-center bg-slate-50 p-4">
            <Suspense fallback={<div>Loading...</div>}>
                <RegisterForm />
            </Suspense>
        </div>
    );
}
