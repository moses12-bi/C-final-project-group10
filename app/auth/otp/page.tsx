'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Mail, ArrowRight, CheckCircle2, AlertCircle } from 'lucide-react';
import api from '@/services/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';

export default function OtpPage() {
    const [step, setStep] = useState<'email' | 'otp'>('email');
    const [email, setEmail] = useState('');
    const [otp, setOtp] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);

    const handleSendOtp = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setMessage(null);

        try {
            await api.post('/auth/send-otp', { email });
            setStep('otp');
            setMessage({ type: 'success', text: 'OTP sent to your email.' });
        } catch (err: any) {
            setMessage({ type: 'error', text: err.response?.data?.message || 'Failed to send OTP.' });
        } finally {
            setIsLoading(false);
        }
    };

    const handleVerifyOtp = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setMessage(null);

        try {
            await api.post('/auth/verify-otp', { email, otp });
            setMessage({ type: 'success', text: 'OTP verified successfully! You can now proceed.' });
        } catch (err: any) {
            setMessage({ type: 'error', text: err.response?.data || 'Invalid OTP.' });
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="flex min-h-screen items-center justify-center bg-dark-950 p-4">
            <Card className="w-full max-w-md border-dark-800 bg-dark-900 shadow-xl">
                <CardHeader>
                    <CardTitle className="text-2xl font-bold text-white">
                        {step === 'email' ? 'Email Verification' : 'Enter OTP'}
                    </CardTitle>
                    <CardDescription className="text-slate-400">
                        {step === 'email'
                            ? 'Enter your email to receive a one-time password'
                            : `Enter the code sent to ${email}`}
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {message && (
                        <div className={`mb-4 flex items-center gap-2 rounded-md p-3 text-sm ${message.type === 'success' ? 'bg-green-500/10 text-green-400' : 'bg-red-500/10 text-red-400'
                            }`}>
                            {message.type === 'success' ? <CheckCircle2 className="h-4 w-4" /> : <AlertCircle className="h-4 w-4" />}
                            {message.text}
                        </div>
                    )}

                    {step === 'email' ? (
                        <form onSubmit={handleSendOtp} className="space-y-4">
                            <div className="space-y-2">
                                <label className="text-sm font-medium text-slate-300">Email Address</label>
                                <div className="relative">
                                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-500" />
                                    <Input
                                        type="email"
                                        required
                                        placeholder="you@example.com"
                                        className="pl-10 bg-dark-950 border-dark-700 text-slate-200 focus:ring-primary-500"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                    />
                                </div>
                            </div>
                            <Button type="submit" className="w-full" isLoading={isLoading}>
                                Send Code
                                <ArrowRight className="ml-2 h-4 w-4" />
                            </Button>
                        </form>
                    ) : (
                        <form onSubmit={handleVerifyOtp} className="space-y-4">
                            <div className="space-y-2">
                                <label className="text-sm font-medium text-slate-300">Verification Code</label>
                                <Input
                                    type="text"
                                    required
                                    placeholder="Enter 6-digit code"
                                    className="bg-dark-950 border-dark-700 text-slate-200 focus:ring-primary-500 text-center text-2xl tracking-widest"
                                    maxLength={6}
                                    value={otp}
                                    onChange={(e) => setOtp(e.target.value.replace(/[^0-9]/g, ''))}
                                />
                            </div>
                            <Button type="submit" className="w-full" isLoading={isLoading}>
                                Verify Code
                            </Button>
                            <button
                                type="button"
                                onClick={() => {
                                    setStep('email');
                                    setMessage(null);
                                    setOtp('');
                                }}
                                className="w-full text-sm text-slate-400 hover:text-white transition-colors"
                            >
                                Change Email
                            </button>
                        </form>
                    )}

                    <div className="mt-6 text-center text-sm">
                        <Link href="/login" className="text-primary-400 hover:text-primary-300">
                            Back to Login
                        </Link>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
}
