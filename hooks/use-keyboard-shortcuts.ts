'use client';

import { useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';

interface KeyboardShortcut {
    key: string;
    ctrl?: boolean;
    shift?: boolean;
    alt?: boolean;
    action: () => void;
    description: string;
}

export function useKeyboardShortcuts(shortcuts: KeyboardShortcut[]) {
    const handleKeyPress = useCallback((event: KeyboardEvent) => {
        for (const shortcut of shortcuts) {
            const ctrlMatch = shortcut.ctrl ? event.ctrlKey || event.metaKey : !event.ctrlKey && !event.metaKey;
            const shiftMatch = shortcut.shift ? event.shiftKey : !event.shiftKey;
            const altMatch = shortcut.alt ? event.altKey : !event.altKey;

            if (
                event.key.toLowerCase() === shortcut.key.toLowerCase() &&
                ctrlMatch &&
                shiftMatch &&
                altMatch
            ) {
                event.preventDefault();
                shortcut.action();
                break;
            }
        }
    }, [shortcuts]);

    useEffect(() => {
        window.addEventListener('keydown', handleKeyPress);
        return () => window.removeEventListener('keydown', handleKeyPress);
    }, [handleKeyPress]);
}

// Predefined app-wide shortcuts
export function useAppKeyboardShortcuts() {
    const router = useRouter();

    const shortcuts: KeyboardShortcut[] = [
        {
            key: 'k',
            ctrl: true,
            action: () => {
                // Trigger global search
                const searchInput = document.querySelector('[data-search-input]') as HTMLInputElement;
                searchInput?.focus();
            },
            description: 'Open search'
        },
        {
            key: 'd',
            ctrl: true,
            action: () => router.push('/dashboard'),
            description: 'Go to dashboard'
        },
        {
            key: 'p',
            ctrl: true,
            action: () => router.push('/projects'),
            description: 'Go to projects'
        },
        {
            key: 'n',
            ctrl: true,
            shift: true,
            action: () => {
                // Create new project/task
                const newButton = document.querySelector('[data-new-button]') as HTMLButtonElement;
                newButton?.click();
            },
            description: 'Create new'
        }
    ];

    useKeyboardShortcuts(shortcuts);
}
