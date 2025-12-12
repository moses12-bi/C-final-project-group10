'use client';

import { useEffect, useState } from 'react';
import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Card, CardContent } from '@/components/ui/card';
import { globalSearch } from '@/services/analytics';
import Link from 'next/link';

export function GlobalSearch() {
    const [query, setQuery] = useState('');
    const [results, setResults] = useState<any>(null);
    const [loading, setLoading] = useState(false);
    const [isOpen, setIsOpen] = useState(false);

    useEffect(() => {
        const delayDebounce = setTimeout(async () => {
            if (query.trim().length < 2) {
                setResults(null);
                return;
            }

            setLoading(true);
            try {
                const data = await globalSearch(query);
                setResults(data);
                setIsOpen(true);
            } catch (error) {
                console.error('Search failed', error);
            } finally {
                setLoading(false);
            }
        }, 300);

        return () => clearTimeout(delayDebounce);
    }, [query]);

    const totalResults = results
        ? (results.Projects?.length || 0) + (results.Tasks?.length || 0) + (results.Users?.length || 0)
        : 0;

    return (
        <div className="relative w-full max-w-md">
            <div className="relative">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
                <Input
                    className="pl-9"
                    placeholder="Search projects, tasks, users..."
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    onFocus={() => results && setIsOpen(true)}
                />
            </div>

            {isOpen && results && (
                <>
                    {/* Backdrop */}
                    <div
                        className="fixed inset-0 z-10"
                        onClick={() => setIsOpen(false)}
                    />

                    {/* Results */}
                    <div className="absolute z-20 mt-2 w-full max-w-2xl bg-white rounded-lg shadow-lg border border-slate-200 max-h-96 overflow-auto">
                        {loading ? (
                            <div className="p-8 text-center text-slate-500">Searching...</div>
                        ) : totalResults === 0 ? (
                            <div className="p-8 text-center text-slate-500">No results found</div>
                        ) : (
                            <div className="p-4 space-y-4">
                                {/* Projects */}
                                {results.Projects?.length > 0 && (
                                    <div>
                                        <div className="text-xs font-semibold text-slate-500 uppercase mb-2">
                                            Projects ({results.Projects.length})
                                        </div>
                                        {results.Projects.map((project: any) => (
                                            <Link
                                                key={project.Id}
                                                href={`/projects/${project.Id}`}
                                                onClick={() => setIsOpen(false)}
                                                className="block p-3 hover:bg-slate-50 rounded-md"
                                            >
                                                <div className="font-medium text-slate-900">{project.Title}</div>
                                                <div className="text-sm text-slate-600 line-clamp-1">{project.Description}</div>
                                            </Link>
                                        ))}
                                    </div>
                                )}

                                {/* Tasks */}
                                {results.Tasks?.length > 0 && (
                                    <div>
                                        <div className="text-xs font-semibold text-slate-500 uppercase mb-2">
                                            Tasks ({results.Tasks.length})
                                        </div>
                                        {results.Tasks.map((task: any) => (
                                            <Link
                                                key={task.Id}
                                                href={`/tasks/${task.Id}`}
                                                onClick={() => setIsOpen(false)}
                                                className="block p-3 hover:bg-slate-50 rounded-md"
                                            >
                                                <div className="font-medium text-slate-900">{task.Title}</div>
                                                <div className="text-sm text-slate-600 line-clamp-1">{task.Description}</div>
                                            </Link>
                                        ))}
                                    </div>
                                )}

                                {/* Users */}
                                {results.Users?.length > 0 && (
                                    <div>
                                        <div className="text-xs font-semibold text-slate-500 uppercase mb-2">
                                            Users ({results.Users.length})
                                        </div>
                                        {results.Users.map((user: any) => (
                                            <div key={user.Id} className="p-3 bg-slate-50 rounded-md">
                                                <div className="font-medium text-slate-900">{user.FullName}</div>
                                                <div className="text-sm text-slate-600">{user.Email} • {user.Role}</div>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        )}
                    </div>
                </>
            )}
        </div>
    );
}
