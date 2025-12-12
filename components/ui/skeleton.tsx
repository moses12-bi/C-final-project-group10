'use client';

import * as React from 'react';

export function LoadingSkeleton({ className }: { className?: string }) {
    return (
        <div className={`animate-pulse bg-slate-200 rounded ${className}`} />
    );
}

export function ProjectCardSkeleton() {
    return (
        <div className="rounded-md border border-slate-200 bg-white p-4">
            <div className="flex items-start justify-between gap-4">
                <div className="flex-1 space-y-2">
                    <LoadingSkeleton className="h-5 w-3/4" />
                    <LoadingSkeleton className="h-4 w-full" />
                </div>
                <LoadingSkeleton className="h-6 w-20" />
            </div>
        </div>
    );
}

export function TaskCardSkeleton() {
    return (
        <div className="rounded-md border border-slate-200 bg-white p-4">
            <div className="flex items-start justify-between gap-4">
                <div className="flex-1 space-y-2">
                    <LoadingSkeleton className="h-5 w-2/3" />
                    <LoadingSkeleton className="h-4 w-full" />
                </div>
                <div className="space-y-1">
                    <LoadingSkeleton className="h-4 w-16" />
                    <LoadingSkeleton className="h-4 w-16" />
                </div>
            </div>
        </div>
    );
}

export function TableSkeleton({ rows = 5 }: { rows?: number }) {
    return (
        <div className="space-y-2">
            <LoadingSkeleton className="h-10 w-full" />
            {Array.from({ length: rows }).map((_, i) => (
                <LoadingSkeleton key={i} className="h-12 w-full" />
            ))}
        </div>
    );
}
