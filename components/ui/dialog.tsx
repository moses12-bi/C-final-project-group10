'use client';

import * as React from 'react';
import { X } from 'lucide-react';
import { Button } from './button';

export interface DialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    children: React.ReactNode;
}

export function Dialog({ open, onOpenChange, children }: DialogProps) {
    if (!open) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
            {/* Backdrop */}
            <div
                className="fixed inset-0 bg-black/60 backdrop-blur-sm transition-opacity"
                onClick={() => onOpenChange(false)}
            />

            {/* Dialog */}
            <div className="relative z-50 w-full max-w-lg mx-4 animate-scale-up">
                {children}
            </div>
        </div>
    );
}

export function DialogContent({ children }: { children: React.ReactNode }) {
    return (
        <div className="relative bg-dark-900 rounded-lg shadow-2xl ring-1 ring-dark-800 border border-dark-800">
            {children}
        </div>
    );
}

export function DialogHeader({ children }: { children: React.ReactNode }) {
    return (
        <div className="flex items-center justify-between border-b border-dark-800 p-6 pb-4">
            {children}
        </div>
    );
}

export function DialogTitle({ children }: { children: React.ReactNode }) {
    return (
        <h2 className="text-lg font-semibold text-white">
            {children}
        </h2>
    );
}

export function DialogClose({ onClose }: { onClose: () => void }) {
    return (
        <button
            onClick={onClose}
            className="rounded-sm opacity-70 ring-offset-dark-950 transition-opacity hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 text-slate-400 hover:text-white"
        >
            <X className="h-4 w-4" />
            <span className="sr-only">Close</span>
        </button>
    );
}

export function DialogBody({ children }: { children: React.ReactNode }) {
    return (
        <div className="p-6">
            {children}
        </div>
    );
}

export function DialogFooter({ children }: { children: React.ReactNode }) {
    return (
        <div className="flex items-center justify-end gap-3 border-t border-dark-800 p-6 pt-4">
            {children}
        </div>
    );
}

// Alert Dialog for confirmations
export interface AlertDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onConfirm: () => void;
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    variant?: 'danger' | 'default';
}

export function AlertDialog({
    open,
    onOpenChange,
    onConfirm,
    title,
    description,
    confirmText = 'Confirm',
    cancelText = 'Cancel',
    variant = 'default'
}: AlertDialogProps) {
    if (!open) return null;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{title}</DialogTitle>
                    <DialogClose onClose={() => onOpenChange(false)} />
                </DialogHeader>
                <DialogBody>
                    <p className="text-sm text-slate-300">{description}</p>
                </DialogBody>
                <DialogFooter>
                    <Button variant="outline" onClick={() => onOpenChange(false)} className="border-dark-700 bg-dark-900 text-slate-200 hover:bg-dark-800">
                        {cancelText}
                    </Button>
                    <Button
                        variant={variant === 'danger' ? 'destructive' : 'default'}
                        onClick={() => {
                            onConfirm();
                            onOpenChange(false);
                        }}
                    >
                        {confirmText}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
