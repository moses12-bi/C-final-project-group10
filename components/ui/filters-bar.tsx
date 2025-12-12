'use client';

import { Filter } from 'lucide-react';
import { Button } from './button';

export interface FilterOption {
    value: string;
    label: string;
}

export interface FilterProps {
    label: string;
    options: FilterOption[];
    value: string;
    onChange: (value: string) => void;
}

export interface FiltersBarProps {
    filters: FilterProps[];
    onReset?: () => void;
}

export function FiltersBar({ filters, onReset }: FiltersBarProps) {
    return (
        <div className="flex flex-wrap items-center gap-3 p-4 bg-white rounded-lg border border-slate-200">
            <div className="flex items-center gap-2 text-sm font-medium text-slate-700">
                <Filter className="h-4 w-4" />
                Filters:
            </div>

            {filters.map((filter, index) => (
                <div key={index} className="flex items-center gap-2">
                    <label className="text-sm text-slate-600">{filter.label}:</label>
                    <select
                        value={filter.value}
                        onChange={(e) => filter.onChange(e.target.value)}
                        className="rounded-md border border-slate-200 px-3 py-1.5 text-sm"
                    >
                        {filter.options.map(option => (
                            <option key={option.value} value={option.value}>
                                {option.label}
                            </option>
                        ))}
                    </select>
                </div>
            ))}

            {onReset && (
                <Button variant="ghost" size="sm" onClick={onReset}>
                    Reset
                </Button>
            )}
        </div>
    );
}
