'use client';

import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Bell, Mail, MessageSquare } from 'lucide-react';
import api from '@/services/api';

type NotificationPreferences = {
    emailNotifications: boolean;
    taskAssignments: boolean;
    taskComments: boolean;
    projectUpdates: boolean;
    teamChanges: boolean;
};

export default function NotificationPreferencesPage() {
    const [preferences, setPreferences] = useState<NotificationPreferences>({
        emailNotifications: true,
        taskAssignments: true,
        taskComments: true,
        projectUpdates: false,
        teamChanges: true
    });
    const [saving, setSaving] = useState(false);

    const handleToggle = (key: keyof NotificationPreferences) => {
        setPreferences(prev => ({ ...prev, [key]: !prev[key] }));
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            await api.put('/users/me/notification-preferences', preferences);
            alert('Preferences saved successfully');
        } catch (error) {
            console.error('Failed to save preferences', error);
            alert('Failed to save preferences');
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-900 p-4 sm:p-8">
            <div className="max-w-3xl mx-auto">
                <h1 className="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-slate-100 mb-6">
                    Notification Preferences
                </h1>

                <Card className="dark:bg-slate-800 dark:border-slate-700">
                    <CardHeader>
                        <CardTitle className="dark:text-slate-100">Configure Notifications</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-6">
                        <div className="space-y-4">
                            <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <Mail className="h-5 w-5 text-slate-600 dark:text-slate-400" />
                                    <div>
                                        <div className="font-medium dark:text-slate-100">Email Notifications</div>
                                        <div className="text-sm text-slate-600 dark:text-slate-400">
                                            Receive notifications via email
                                        </div>
                                    </div>
                                </div>
                                <input
                                    type="checkbox"
                                    checked={preferences.emailNotifications}
                                    onChange={() => handleToggle('emailNotifications')}
                                    className="h-4 w-4"
                                />
                            </div>

                            <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <Bell className="h-5 w-5 text-slate-600 dark:text-slate-400" />
                                    <div>
                                        <div className="font-medium dark:text-slate-100">Task Assignments</div>
                                        <div className="text-sm text-slate-600 dark:text-slate-400">
                                            When you're assigned to a task
                                        </div>
                                    </div>
                                </div>
                                <input
                                    type="checkbox"
                                    checked={preferences.taskAssignments}
                                    onChange={() => handleToggle('taskAssignments')}
                                    className="h-4 w-4"
                                />
                            </div>

                            <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <MessageSquare className="h-5 w-5 text-slate-600 dark:text-slate-400" />
                                    <div>
                                        <div className="font-medium dark:text-slate-100">Task Comments</div>
                                        <div className="text-sm text-slate-600 dark:text-slate-400">
                                            When someone comments on your tasks
                                        </div>
                                    </div>
                                </div>
                                <input
                                    type="checkbox"
                                    checked={preferences.taskComments}
                                    onChange={() => handleToggle('taskComments')}
                                    className="h-4 w-4"
                                />
                            </div>

                            <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <Bell className="h-5 w-5 text-slate-600 dark:text-slate-400" />
                                    <div>
                                        <div className="font-medium dark:text-slate-100">Project Updates</div>
                                        <div className="text-sm text-slate-600 dark:text-slate-400">
                                            When projects are updated
                                        </div>
                                    </div>
                                </div>
                                <input
                                    type="checkbox"
                                    checked={preferences.projectUpdates}
                                    onChange={() => handleToggle('projectUpdates')}
                                    className="h-4 w-4"
                                />
                            </div>

                            <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                                <div className="flex items-center gap-3">
                                    <Bell className="h-5 w-5 text-slate-600 dark:text-slate-400" />
                                    <div>
                                        <div className="font-medium dark:text-slate-100">Team Changes</div>
                                        <div className="text-sm text-slate-600 dark:text-slate-400">
                                            When team members are added/removed
                                        </div>
                                    </div>
                                </div>
                                <input
                                    type="checkbox"
                                    checked={preferences.teamChanges}
                                    onChange={() => handleToggle('teamChanges')}
                                    className="h-4 w-4"
                                />
                            </div>
                        </div>

                        <div className="pt-4">
                            <Button onClick={handleSave} isLoading={saving}>
                                Save Preferences
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
