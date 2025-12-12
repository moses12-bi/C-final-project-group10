'use client';

import { useEffect } from 'react';
import { AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/button';

export default function Error({
    error,
    reset,
}: {
    error: Error & { digest?: string };
    reset: () => void;
}) {
    useEffect(() => {
        console.error('Error boundary caught:', error);
    }, [error]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-slate-50">
            <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-8 text-center">
                <AlertTriangle className="h-16 w-16 text-red-500 mx-auto mb-4" />
                <h2 className="text-2xl font-bold text-slate-900 mb-2">
                    Something went wrong!
                </h2>
                <p className="text-slate-600 mb-6">
                    {error.message || 'An unexpected error occurred'}
                </p>
                <div className="flex gap-3 justify-center">
                    <Button onClick={() => window.location.href = '/'} variant="outline">
                        Go Home
                    </Button>
                    <Button onClick={reset}>
                        Try Again
                    </Button>
                </div>
            </div>
        </div>
    );
}
