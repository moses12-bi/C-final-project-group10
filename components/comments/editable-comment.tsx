'use client';

import { useState } from 'react';
import { Pencil, Trash2, X, Check } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { updateComment, deleteComment } from '@/services/comments.update';

interface EditableCommentProps {
    comment: {
        id: number;
        taskId: number;
        content: string;
        userFullName: string;
        userId: string;
        createdAt: string;
    };
    currentUserId: string;
    onUpdate: () => void;
}

export function EditableComment({ comment, currentUserId, onUpdate }: EditableCommentProps) {
    const [editing, setEditing] = useState(false);
    const [content, setContent] = useState(comment.content);
    const [saving, setSaving] = useState(false);

    const isOwner = comment.userId === currentUserId;

    const handleSave = async () => {
        setSaving(true);
        try {
            await updateComment(comment.taskId, comment.id, content);
            setEditing(false);
            onUpdate();
        } catch (error) {
            console.error('Failed to update comment', error);
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!confirm('Delete this comment?')) return;

        try {
            await deleteComment(comment.taskId, comment.id);
            onUpdate();
        } catch (error) {
            console.error('Failed to delete comment', error);
        }
    };

    return (
        <div className="border-l-2 border-slate-200 dark:border-slate-700 pl-4 mb-4">
            <div className="flex items-start justify-between">
                <div>
                    <div className="font-medium text-sm dark:text-slate-100">{comment.userFullName}</div>
                    <div className="text-xs text-slate-500 dark:text-slate-400">
                        {new Date(comment.createdAt).toLocaleString()}
                    </div>
                </div>
                {isOwner && !editing && (
                    <div className="flex gap-1">
                        <button
                            onClick={() => setEditing(true)}
                            className="p-1 text-slate-400 hover:text-blue-600"
                        >
                            <Pencil className="h-3 w-3" />
                        </button>
                        <button
                            onClick={handleDelete}
                            className="p-1 text-slate-400 hover:text-red-600"
                        >
                            <Trash2 className="h-3 w-3" />
                        </button>
                    </div>
                )}
            </div>

            {editing ? (
                <div className="mt-2">
                    <textarea
                        className="w-full rounded-md border border-slate-200 dark:border-slate-600 dark:bg-slate-700 dark:text-slate-100 p-2 text-sm resize-none"
                        rows={3}
                        value={content}
                        onChange={(e) => setContent(e.target.value)}
                    />
                    <div className="flex gap-2 mt-2">
                        <Button size="sm" onClick={handleSave} isLoading={saving}>
                            <Check className="h-3 w-3 mr-1" />
                            Save
                        </Button>
                        <Button size="sm" variant="outline" onClick={() => {
                            setContent(comment.content);
                            setEditing(false);
                        }}>
                            <X className="h-3 w-3 mr-1" />
                            Cancel
                        </Button>
                    </div>
                </div>
            ) : (
                <p className="mt-2 text-slate-700 dark:text-slate-300">{comment.content}</p>
            )}
        </div>
    );
}
