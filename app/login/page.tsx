'use client';

import { Suspense, useState } from 'react';
import Link from 'next/link';
import { useRouter, useSearchParams } from 'next/navigation';
import api from '@/services/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

function LoginForm() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const justRegistered = searchParams.get('registered') === 'true';

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await api.post('/auth/login', { email, password });

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
            console.error(err);
            const maybe = err as { response?: { data?: unknown } };
            if (maybe.response && maybe.response.data) {
                setError(typeof maybe.response.data === 'string' ? maybe.response.data : 'Invalid credentials');
            } else {
                setError('Something went wrong. Please try again.');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="flex min-h-screen flex-col items-center justify-center bg-dark-950 p-4">
            <div className="w-full max-w-md space-y-8 rounded-xl bg-dark-900 p-8 shadow-xl ring-1 ring-dark-800 border border-dark-800">
                <div className="text-center">
                    <h1 className="text-3xl font-bold tracking-tight text-white">ProjectM</h1>
                    <p className="mt-2 text-sm text-slate-400">
                        Sign in to your account
                    </p>
                </div>

                <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
                    {justRegistered && (
                        <div className="rounded-md bg-green-500/10 p-3 text-sm text-green-400 ring-1 ring-green-500/20">
                            ✓ Registration successful! Please sign in with your new account.
                        </div>
                    )}
                    {error && (
                        <div className="rounded-md bg-red-500/10 p-3 text-sm text-red-400 ring-1 ring-red-500/20">
                            {error}
                        </div>
                    )}

                    <div className="space-y-4">
                        <div>
                            <label htmlFor="email" className="block text-sm font-medium text-slate-300">
                                Email address
                            </label>
                            <div className="mt-1">
                                <Input
                                    id="email"
                                    name="email"
                                    type="email"
                                    autoComplete="email"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    disabled={isLoading}
                                    placeholder="name@company.com"
                                    className="w-full"
                                />
                            </div>
                        </div>

                        <div>
                            <label htmlFor="password" className="block text-sm font-medium text-slate-300">
                                Password
                            </label>
                            <div className="mt-1">
                                <Input
                                    id="password"
                                    name="password"
                                    type="password"
                                    autoComplete="current-password"
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    disabled={isLoading}
                                    placeholder="••••••••"
                                    className="w-full"
                                />
                            </div>
                        </div>
                    </div>

                    <div>
                        <Button
                            type="submit"
                            className="w-full shadow-lg shadow-primary-500/20"
                            isLoading={isLoading}
                        >
                            Sign in
                        </Button>
                    </div>

                    <div className="text-center text-sm">
                        <span className="text-slate-500">Don&apos;t have an account? </span>
                        <Link href="/register" className="font-semibold text-primary-400 hover:text-primary-300">
                            Complete Invitation
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default function LoginPage() {
    return (
        <Suspense fallback={<div>Loading...</div>}>
            <LoginForm />
        </Suspense>
    );
}
