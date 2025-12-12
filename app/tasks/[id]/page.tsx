'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { ArrowLeft, Users, Paperclip, MessageSquare } from 'lucide-react';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { hasPermission } from '@/lib/permissions';
import { getStoredUser } from '@/lib/session';
import { listComments, createComment, type Comment } from '@/services/comments';
import { listAttachments, uploadAttachment, downloadAttachment, type Attachment } from '@/services/attachments';
import { listAssignments, assignUser, unassignUser, type TaskAssignment } from '@/services/assignments';
import { ProjectTask, TaskPriority, TaskStatus } from '@/services/tasks';
import { UserPicker, type User } from '@/components/ui/user-picker';
import { listAllUsers } from '@/services/users';

export default function TaskDetailPage() {
    const params = useParams();
    const router = useRouter();
    const taskId = parseInt(params.id as string);

    const [task, setTask] = useState<ProjectTask | null>(null);
    const [comments, setComments] = useState<Comment[]>([]);
    const [attachments, setAttachments] = useState<Attachment[]>([]);
    const [assignments, setAssignments] = useState<TaskAssignment[]>([]);
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(true);

    const [newComment, setNewComment] = useState('');
    const [commentLoading, setCommentLoading] = useState(false);
    const [uploadingFile, setUploadingFile] = useState(false);

    const canRead = hasPermission('tasks.read');
    const canWrite = hasPermission('tasks.write');
    const currentUser = getStoredUser();

    useEffect(() => {
        loadTaskData();
    }, [taskId]);

    const loadTaskData = async () => {
        setLoading(true);
        try {
            // Load all task data in parallel
            const [commentsData, attachmentsData, assignmentsData, usersData] = await Promise.all([
                listComments(taskId),
                listAttachments(taskId),
                listAssignments(taskId),
                listAllUsers()
            ]);

            setComments(commentsData);
            setAttachments(attachmentsData);
            setAssignments(assignmentsData);
            setUsers(usersData);
        } catch (error) {
            console.error('Failed to load task data', error);
        } finally {
            setLoading(false);
        }
    };

    const handleAddComment = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newComment.trim() || !canWrite) return;

        setCommentLoading(true);
        try {
            await createComment(taskId, { content: newComment });
            setNewComment('');
            await loadTaskData();
        } catch (error) {
            console.error('Failed to add comment', error);
        } finally {
            setCommentLoading(false);
        }
    };

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file || !canWrite) return;

        setUploadingFile(true);
        try {
            await uploadAttachment(taskId, file);
            await loadTaskData();
        } catch (error) {
            console.error('Failed to upload file', error);
        } finally {
            setUploadingFile(false);
        }
    };

    const handleDownload = async (attachmentId: number, fileName: string) => {
        try {
            const blob = await downloadAttachment(taskId, attachmentId);
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        } catch (error) {
            console.error('Failed to download file', error);
        }
    };

    const handleAssignUser = async (userId: string) => {
        try {
            await assignUser(taskId, { userId });
            await loadTaskData();
        } catch (error) {
            console.error('Failed to assign user', error);
        }
    };

    const handleUnassignUser = async (userId: string) => {
        try {
            await unassignUser(taskId, userId);
            await loadTaskData();
        } catch (error) {
            console.error('Failed to unassign user', error);
        }
    };

    if (!canRead) {
        return (
            <div className="min-h-screen bg-slate-50/50 p-8">
                <div className="text-center text-slate-600">
                    You do not have permission to view tasks.
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50/50 p-8">
            <div className="max-w-6xl mx-auto">
                <Button variant="ghost" onClick={() => router.back()} className="mb-4">
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Back
                </Button>

                <div className="grid grid-cols-3 gap-6">
                    {/* Main Content */}
                    <div className="col-span-2 space-y-6">
                        {/* Comments */}
                        <Card>
                            <CardHeader>
                                <CardTitle className="flex items-center gap-2">
                                    <MessageSquare className="h-5 w-5" />
                                    Comments ({comments.length})
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="space-y-4">
                                    {comments.map((comment) => (
                                        <div key={comment.id} className="border-l-2 border-slate-200 pl-4">
                                            <div className="flex items-start justify-between">
                                                <div>
                                                    <div className="font-medium text-sm">{comment.userFullName}</div>
                                                    <div className="text-xs text-slate-500">
                                                        {new Date(comment.createdAt).toLocaleString()}
                                                    </div>
                                                </div>
                                            </div>
                                            <p className="mt-2 text-slate-700">{comment.content}</p>
                                        </div>
                                    ))}

                                    {canWrite && (
                                        <form onSubmit={handleAddComment} className="mt-4">
                                            <textarea
                                                className="w-full rounded-md border border-slate-200 p-3 text-sm resize-none"
                                                rows={3}
                                                placeholder="Add a comment..."
                                                value={newComment}
                                                onChange={(e) => setNewComment(e.target.value)}
                                            />
                                            <Button type="submit" className="mt-2" isLoading={commentLoading}>
                                                Add Comment
                                            </Button>
                                        </form>
                                    )}
                                </div>
                            </CardContent>
                        </Card>

                        {/* Attachments */}
                        <Card>
                            <CardHeader>
                                <CardTitle className="flex items-center gap-2">
                                    <Paperclip className="h-5 w-5" />
                                    Attachments ({attachments.length})
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="space-y-2">
                                    {attachments.map((attachment) => (
                                        <div
                                            key={attachment.id}
                                            className="flex items-center justify-between p-3 bg-slate-50 rounded-md"
                                        >
                                            <div className="flex-1 min-w-0">
                                                <div className="font-medium text-sm truncate">{attachment.fileName}</div>
                                                <div className="text-xs text-slate-500">
                                                    {(attachment.fileSize / 1024).toFixed(1)} KB • {attachment.uploadedBy}
                                                </div>
                                            </div>
                                            <Button
                                                size="sm"
                                                variant="outline"
                                                onClick={() => handleDownload(attachment.id, attachment.fileName)}
                                            >
                                                Download
                                            </Button>
                                        </div>
                                    ))}

                                    {canWrite && (
                                        <div className="mt-4">
                                            <input
                                                type="file"
                                                id="file-upload"
                                                className="hidden"
                                                onChange={handleFileUpload}
                                                disabled={uploadingFile}
                                            />
                                            <Button
                                                variant="outline"
                                                onClick={() => document.getElementById('file-upload')?.click()}
                                                isLoading={uploadingFile}
                                            >
                                                <Paperclip className="mr-2 h-4 w-4" />
                                                Upload File
                                            </Button>
                                        </div>
                                    )}
                                </div>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Sidebar */}
                    <div className="space-y-6">
                        {/* Assignees */}
                        <Card>
                            <CardHeader>
                                <CardTitle className="flex items-center gap-2">
                                    <Users className="h-5 w-5" />
                                    Assigned To
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="space-y-3">
                                    {assignments.map((assignment) => (
                                        <div
                                            key={assignment.userId}
                                            className="flex items-center justify-between p-2 bg-slate-50 rounded"
                                        >
                                            <div className="text-sm">{assignment.userFullName}</div>
                                            {canWrite && (
                                                <button
                                                    onClick={() => handleUnassignUser(assignment.userId)}
                                                    className="text-red-600 hover:text-red-700 text-xs"
                                                >
                                                    Remove
                                                </button>
                                            )}
                                        </div>
                                    ))}

                                    {canWrite && (
                                        <div className="mt-3">
                                            <UserPicker
                                                users={users}
                                                selectedUsers={assignments.map(a => a.userId)}
                                                onSelect={handleAssignUser}
                                                onDeselect={handleUnassignUser}
                                                placeholder="Assign user..."
                                            />
                                        </div>
                                    )}
                                </div>
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
}
