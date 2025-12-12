'use client';

import * as React from 'react';
import { useState, useEffect } from 'react';
import { Search, Check } from 'lucide-react';
import { Input } from './input';
import { Button } from './button';

export type User = {
    id: string;
    fullName: string;
    email: string;
    role: string;
    department?: string | null;
};

export interface UserPickerProps {
    users: User[];
    selectedUsers?: string[];
    onSelect: (userId: string) => void;
    onDeselect?: (userId: string) => void;
    singleSelect?: boolean;
    placeholder?: string;
}

export function UserPicker({
    users,
    selectedUsers = [],
    onSelect,
    onDeselect,
    singleSelect = false,
    placeholder = 'Search users...'
}: UserPickerProps) {
    const [search, setSearch] = useState('');
    const [isOpen, setIsOpen] = useState(false);

    const filteredUsers = users.filter(user =>
        user.fullName.toLowerCase().includes(search.toLowerCase()) ||
        user.email.toLowerCase().includes(search.toLowerCase())
    );

    const handleSelect = (userId: string) => {
        if (selectedUsers.includes(userId)) {
            onDeselect?.(userId);
        } else {
            onSelect(userId);
            if (singleSelect) {
                setIsOpen(false);
                setSearch('');
            }
        }
    };

    return (
        <div className="relative">
            <div className="relative">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
                <Input
                    className="pl-9"
                    placeholder={placeholder}
                    value={search}
                    onChange={(e) => {
                        setSearch(e.target.value);
                        setIsOpen(true);
                    }}
                    onFocus={() => setIsOpen(true)}
                />
            </div>

            {isOpen && (
                <>
                    {/* Backdrop */}
                    <div
                        className="fixed inset-0 z-10"
                        onClick={() => {
                            setIsOpen(false);
                            setSearch('');
                        }}
                    />

                    {/* Dropdown */}
                    <div className="absolute z-20 mt-1 w-full rounded-md border border-slate-200 bg-white shadow-lg max-h-60 overflow-auto">
                        {filteredUsers.length === 0 ? (
                            <div className="p-4 text-sm text-slate-500 text-center">
                                No users found
                            </div>
                        ) : (
                            <div className="py-1">
                                {filteredUsers.map(user => (
                                    <button
                                        key={user.id}
                                        type="button"
                                        onClick={() => handleSelect(user.id)}
                                        className="w-full flex items-center gap-3 px-4 py-2 text-left hover:bg-slate-100 transition-colors"
                                    >
                                        <div className="flex-1 min-w-0">
                                            <div className="font-medium text-slate-900 truncate">
                                                {user.fullName}
                                            </div>
                                            <div className="text-sm text-slate-500 truncate">
                                                {user.email} • {user.role}
                                            </div>
                                        </div>
                                        {selectedUsers.includes(user.id) && (
                                            <Check className="h-4 w-4 text-slate-900 flex-shrink-0" />
                                        )}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </>
            )}
        </div>
    );
}
