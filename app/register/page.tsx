'use client';

import { Suspense, useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { CheckCircle2, UserPlus } from 'lucide-react';
import Link from 'next/link';
import api from '@/services/api';
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

            // Registration successful - redirect to login page
            if (response.data) {
                router.push('/login?registered=true');
            }
        } catch (err: any) {
            console.error('Registration error:', err);
            const errorMessage = err.response?.data?.message
                || err.response?.data
                || err.message
                || 'Registration failed. Please try again.';
            setError(typeof errorMessage === 'string' ? errorMessage : JSON.stringify(errorMessage));
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
        <div className="w-full max-w-md space-y-8 rounded-xl bg-dark-900 p-8 shadow-xl ring-1 ring-dark-800 border border-dark-800">
            <div className="text-center">
                <h1 className="text-3xl font-bold tracking-tight text-white">
                    {isInviteMode ? 'Complete Setup' : 'Create Account'}
                </h1>
                <p className="mt-2 text-sm text-slate-400">
                    {isInviteMode && inviteData ? (
                        <>Welcome, <strong className="text-slate-200">{inviteData.email}</strong>!</>
                    ) : (
                        'Get started with ProjectM'
                    )}
                </p>
                {isInviteMode && inviteData && (
                    <div className="mt-2 flex items-center justify-center gap-2 text-xs text-slate-500">
                        <span className="inline-flex items-center rounded-full bg-secondary-900/50 px-2 py-1 text-secondary-400 ring-1 ring-inset ring-secondary-500/20">{inviteData.department}</span>
                        <span className="inline-flex items-center rounded-full bg-primary-900/50 px-2 py-1 text-primary-400 ring-1 ring-inset ring-primary-500/20">{inviteData.role}</span>
                    </div>
                )}
            </div>

            <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
                {error && (
                    <div className="rounded-md bg-red-500/10 p-3 text-sm text-red-400 ring-1 ring-red-500/20">
                        {error}
                    </div>
                )}

                <div className="space-y-4">
                    {!isInviteMode && (
                        <div>
                            <label className="block text-sm font-medium text-slate-300">Email Address</label>
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
                        <label className="block text-sm font-medium text-slate-300">Full Name</label>
                        <Input
                            required
                            value={fullName}
                            onChange={(e) => setFullName(e.target.value)}
                            placeholder="John Doe"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-300">Password</label>
                        <Input
                            type="password"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="••••••••"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-300">Confirm Password</label>
                        <Input
                            type="password"
                            required
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            placeholder="••••••••"
                        />
                    </div>
                </div>

                <Button type="submit" className="w-full shadow-lg shadow-primary-500/20" isLoading={isLoading}>
                    {isInviteMode ? <CheckCircle2 className="mr-2 h-4 w-4" /> : <UserPlus className="mr-2 h-4 w-4" />}
                    {isInviteMode ? 'Complete Registration' : 'Create Account'}
                </Button>

                {!isInviteMode && (
                    <div className="text-center text-sm">
                        <span className="text-slate-500">Already have an account? </span>
                        <Link href="/login" className="font-semibold text-primary-400 hover:text-primary-300">
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
        <div className="flex min-h-screen flex-col items-center justify-center bg-dark-950 p-4">
            <Suspense fallback={<div>Loading...</div>}>
                <RegisterForm />
            </Suspense>
        </div>
    );
}
