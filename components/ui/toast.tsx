'use client';

import * as React from 'react';
import { X, CheckCircle, AlertCircle, Info, AlertTriangle } from 'lucide-react';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface ToastProps {
    id: string;
    type: ToastType;
    title: string;
    message?: string;
    duration?: number;
    onClose: (id: string) => void;
}

export function Toast({ id, type, title, message, duration = 5000, onClose }: ToastProps) {
    React.useEffect(() => {
        if (duration > 0) {
            const timer = setTimeout(() => onClose(id), duration);
            return () => clearTimeout(timer);
        }
    }, [id, duration, onClose]);

    const icons = {
        success: <CheckCircle className="h-5 w-5 text-green-600" />,
        error: <AlertCircle className="h-5 w-5 text-red-600" />,
        warning: <AlertTriangle className="h-5 w-5 text-yellow-600" />,
        info: <Info className="h-5 w-5 text-blue-600" />
    };

    const styles = {
        success: 'bg-green-50 border-green-200',
        error: 'bg-red-50 border-red-200',
        warning: 'bg-yellow-50 border-yellow-200',
        info: 'bg-blue-50 border-blue-200'
    };

    return (
        <div className={`flex items-start gap-3 p-4 rounded-lg border shadow-lg ${styles[type]} animate-in slide-in-from-right`}>
            {icons[type]}
            <div className="flex-1 min-w-0">
                <div className="font-medium text-slate-900">{title}</div>
                {message && <div className="text-sm text-slate-600 mt-1">{message}</div>}
            </div>
            <button
                onClick={() => onClose(id)}
                className="text-slate-400 hover:text-slate-600 transition-colors"
            >
                <X className="h-4 w-4" />
            </button>
        </div>
    );
}

export interface ToastContainerProps {
    toasts: ToastProps[];
    onClose: (id: string) => void;
}

export function ToastContainer({ toasts, onClose }: ToastContainerProps) {
    return (
        <div className="fixed top-4 right-4 z-50 flex flex-col gap-2 w-96 max-w-full">
            {toasts.map((toast) => (
                <Toast key={toast.id} {...toast} onClose={onClose} />
            ))}
        </div>
    );
}

// Toast Hook
export function useToast() {
    const [toasts, setToasts] = React.useState<ToastProps[]>([]);

    const show = React.useCallback((type: ToastType, title: string, message?: string, duration?: number) => {
        const id = Math.random().toString(36).substring(7);
        setToasts((prev) => [...prev, { id, type, title, message, duration, onClose: remove }]);
    }, []);

    const remove = React.useCallback((id: string) => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
    }, []);

    const success = React.useCallback((title: string, message?: string) => {
        show('success', title, message);
    }, [show]);

    const error = React.useCallback((title: string, message?: string) => {
        show('error', title, message);
    }, [show]);

    const warning = React.useCallback((title: string, message?: string) => {
        show('warning', title, message);
    }, [show]);

    const info = React.useCallback((title: string, message?: string) => {
        show('info', title, message);
    }, [show]);

    return {
        toasts,
        success,
        error,
        warning,
        info,
        remove
    };
}
