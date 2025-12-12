'use client';

import { useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogBody, DialogFooter, DialogClose, AlertDialog } from '@/components/ui/dialog';
import { hasPermission } from '@/lib/permissions';
import { getProject, type Project } from '@/services/projects';
import { createTask, updateTask, deleteTask, listTasks, type ProjectTask, type TaskPriority, type TaskStatus } from '@/services/tasks';

function toIsoDate(d: Date) {
  return d.toISOString().slice(0, 10);
}

export default function ProjectDetailsPage() {
  const params = useParams<{ id: string }>();
  const projectId = Number(params.id);

  const canReadProjects = hasPermission('projects.read');
  const canReadTasks = hasPermission('tasks.read');
  const canWriteTasks = hasPermission('tasks.write');

  const [project, setProject] = useState<Project | null>(null);
  const [tasks, setTasks] = useState<ProjectTask[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  // create/edit task form
  const today = useMemo(() => new Date(), []);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TaskPriority>('Medium');
  const [status, setStatus] = useState<TaskStatus>('ToDo');
  const [startDate, setStartDate] = useState(toIsoDate(today));
  const [deadline, setDeadline] = useState(toIsoDate(new Date(today.getTime() + 1000 * 60 * 60 * 24 * 7)));
  const [estimatedHours, setEstimatedHours] = useState<number>(8);
  const [saving, setSaving] = useState(false);

  // edit/delete state
  const [editingTask, setEditingTask] = useState<ProjectTask | null>(null);
  const [deletingTask, setDeletingTask] = useState<ProjectTask | null>(null);

  const load = async () => {
    if (!canReadProjects) {
      setLoading(false);
      return;
    }

    setLoading(true);
    setError('');
    try {
      const p = await getProject(projectId);
      setProject(p);
      if (canReadTasks) {
        const t = await listTasks(projectId);
        setTasks(t);
      } else {
        setTasks([]);
      }
    } catch {
      setError('Failed to load project/tasks');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!Number.isFinite(projectId)) return;
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [projectId]);

  const onCreateTask = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWriteTasks) return;

    setSaving(true);
    setError('');
    try {
      await createTask(projectId, {
        title,
        description,
        priority,
        status,
        startDate,
        deadline,
        estimatedHours
      });
      setTitle('');
      setDescription('');
      setPriority('Medium');
      setStatus('ToDo');
      await load();
    } catch {
      setError('Failed to create task');
    } finally {
      setSaving(false);
    }
  };

  const openEditDialog = (task: ProjectTask) => {
    setEditingTask(task);
    setTitle(task.title);
    setDescription(task.description || '');
    setPriority(task.priority);
    setStatus(task.status);
    setStartDate(task.startDate);
    setDeadline(task.deadline);
    setEstimatedHours(task.estimatedHours || 8);
  };

  const closeEditDialog = () => {
    setEditingTask(null);
    setTitle('');
    setDescription('');
    setPriority('Medium');
    setStatus('ToDo');
  };

  const onUpdateTask = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWriteTasks || !editingTask) return;

    setSaving(true);
    setError('');
    try {
      await updateTask(projectId, editingTask.id, {
        title,
        description,
        priority,
        status,
        startDate,
        deadline,
        estimatedHours,
        actualHours: editingTask.actualHours
      });
      closeEditDialog();
      await load();
    } catch {
      setError('Failed to update task');
    } finally {
      setSaving(false);
    }
  };

  const onDeleteTask = async () => {
    if (!deletingTask) return;

    setError('');
    try {
      await deleteTask(projectId, deletingTask.id);
      setDeletingTask(null);
      await load();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete task');
      setDeletingTask(null);
    }
  };

  if (!canReadProjects) {
    return (
      <div className="min-h-screen bg-slate-50/50 p-8">
        <Card>
          <CardHeader>
            <CardTitle>Project</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-slate-600">You do not have permission to view projects.</p>
            <div className="mt-4 flex gap-2">
              <Link href="/projects"><Button variant="outline">Back to Projects</Button></Link>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-slate-900">{project?.title ?? 'Project'}</h1>
          <p className="text-slate-500">{project?.description}</p>
        </div>
        <div className="flex gap-2">
          <Link href="/projects"><Button variant="outline">Back</Button></Link>
        </div>
      </div>

      {error && (
        <div className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-600 ring-1 ring-red-200">
          {error}
        </div>
      )}

      {loading ? (
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-900 border-t-transparent" />
      ) : (
        <div className="grid gap-6 lg:grid-cols-3">
          <div className="lg:col-span-2">
            <Card>
              <CardHeader>
                <CardTitle>Tasks</CardTitle>
              </CardHeader>
              <CardContent>
                {!canReadTasks ? (
                  <p className="text-sm text-slate-600">You do not have permission to view tasks.</p>
                ) : tasks.length === 0 ? (
                  <p className="text-sm text-slate-500">No tasks found.</p>
                ) : (
                  <div className="space-y-3">
                    {tasks.map(t => (
                      <div key={t.id} className="rounded-md border border-slate-200 bg-white p-4">
                        <div className="flex items-start justify-between gap-4">
                          <div className="flex-1">
                            <div className="font-medium text-slate-900">{t.title}</div>
                            <div className="text-sm text-slate-500">{t.description}</div>
                          </div>
                          <div className="flex items-center gap-2">
                            <div className="text-right text-xs text-slate-500">
                              <div>{t.status}</div>
                              <div>{t.priority}</div>
                            </div>
                            {canWriteTasks && (
                              <>
                                <Button
                                  size="icon"
                                  variant="ghost"
                                  onClick={() => openEditDialog(t)}
                                >
                                  <Pencil className="h-4 w-4" />
                                </Button>
                                <Button
                                  size="icon"
                                  variant="ghost"
                                  onClick={() => setDeletingTask(t)}
                                >
                                  <Trash2 className="h-4 w-4 text-red-500" />
                                </Button>
                              </>
                            )}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          <div>
            <Card>
              <CardHeader>
                <CardTitle>Create Task</CardTitle>
              </CardHeader>
              <CardContent>
                <form onSubmit={onCreateTask} className="space-y-3">
                  <fieldset disabled={!canWriteTasks || saving} className="space-y-3">
                    {!canWriteTasks && (
                      <div className="rounded-md bg-yellow-50 p-3 text-sm text-yellow-800 ring-1 ring-yellow-200">
                        You do not have permission to create tasks.
                      </div>
                    )}
                    <div>
                      <label className="text-sm font-medium">Title</label>
                      <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
                    </div>
                    <div>
                      <label className="text-sm font-medium">Description</label>
                      <Input value={description} onChange={(e) => setDescription(e.target.value)} required />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="text-sm font-medium">Priority</label>
                        <select
                          className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                          value={priority}
                          onChange={(e) => setPriority(e.target.value as TaskPriority)}
                        >
                          <option value="Low">Low</option>
                          <option value="Medium">Medium</option>
                          <option value="High">High</option>
                          <option value="Critical">Critical</option>
                        </select>
                      </div>
                      <div>
                        <label className="text-sm font-medium">Status</label>
                        <select
                          className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                          value={status}
                          onChange={(e) => setStatus(e.target.value as TaskStatus)}
                        >
                          <option value="ToDo">ToDo</option>
                          <option value="InProgress">InProgress</option>
                          <option value="Review">Review</option>
                          <option value="Done">Done</option>
                          <option value="Block">Block</option>
                        </select>
                      </div>
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="text-sm font-medium">Start</label>
                        <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
                      </div>
                      <div>
                        <label className="text-sm font-medium">Deadline</label>
                        <Input type="date" value={deadline} onChange={(e) => setDeadline(e.target.value)} required />
                      </div>
                    </div>
                    <div>
                      <label className="text-sm font-medium">Estimated Hours</label>
                      <Input
                        type="number"
                        min={0}
                        step={1}
                        value={String(estimatedHours)}
                        onChange={(e) => setEstimatedHours(Number(e.target.value))}
                        required
                      />
                    </div>
                    <Button type="submit" className="w-full" isLoading={saving}>
                      <Plus className="mr-2 h-4 w-4" />
                      Create
                    </Button>
                  </fieldset>
                </form>
              </CardContent>
            </Card>
          </div>
        </div>
      )}

      {/* Edit Task Dialog */}
      <Dialog open={!!editingTask} onOpenChange={(open) => !open && closeEditDialog()}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Task</DialogTitle>
            <DialogClose onClose={closeEditDialog} />
          </DialogHeader>
          <form onSubmit={onUpdateTask}>
            <DialogBody>
              <fieldset disabled={saving} className="space-y-3">
                <div>
                  <label className="text-sm font-medium">Title</label>
                  <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
                </div>
                <div>
                  <label className="text-sm font-medium">Description</label>
                  <Input value={description} onChange={(e) => setDescription(e.target.value)} required />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium">Priority</label>
                    <select
                      className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                      value={priority}
                      onChange={(e) => setPriority(e.target.value as TaskPriority)}
                    >
                      <option value="Low">Low</option>
                      <option value="Medium">Medium</option>
                      <option value="High">High</option>
                      <option value="Critical">Critical</option>
                    </select>
                  </div>
                  <div>
                    <label className="text-sm font-medium">Status</label>
                    <select
                      className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                      value={status}
                      onChange={(e) => setStatus(e.target.value as TaskStatus)}
                    >
                      <option value="ToDo">ToDo</option>
                      <option value="InProgress">InProgress</option>
                      <option value="Review">Review</option>
                      <option value="Done">Done</option>
                      <option value="Block">Block</option>
                    </select>
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium">Start</label>
                    <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
                  </div>
                  <div>
                    <label className="text-sm font-medium">Deadline</label>
                    <Input type="date" value={deadline} onChange={(e) => setDeadline(e.target.value)} required />
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium">Estimated Hours</label>
                  <Input
                    type="number"
                    min={0}
                    step={1}
                    value={String(estimatedHours)}
                    onChange={(e) => setEstimatedHours(Number(e.target.value))}
                    required
                  />
                </div>
              </fieldset>
            </DialogBody>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={closeEditDialog}>
                Cancel
              </Button>
              <Button type="submit" isLoading={saving}>
                Save Changes
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation */}
      <AlertDialog
        open={!!deletingTask}
        onOpenChange={(open) => !open && setDeletingTask(null)}
        onConfirm={onDeleteTask}
        title="Delete Task"
        description={`Are you sure you want to delete "${deletingTask?.title}"? This action cannot be undone.`}
        confirmText="Delete"
        variant="danger"
      />
    </div>
  );
}
