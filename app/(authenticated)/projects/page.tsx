'use client';

import { useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogBody, DialogFooter, DialogClose, AlertDialog } from '@/components/ui/dialog';
import { hasPermission } from '@/lib/permissions';
import { getStoredUser } from '@/lib/session';
import { createProject, updateProject, deleteProject, listProjects, type Project, type ProjectStatus } from '@/services/projects';

function toIsoDate(d: Date) {
  return d.toISOString().slice(0, 10);
}

export default function ProjectsPage() {
  const canRead = hasPermission('projects.read');
  const canWrite = hasPermission('projects.write');

  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');

  // create/edit form
  const today = useMemo(() => new Date(), []);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [goal, setGoal] = useState('');
  const [status, setStatus] = useState<ProjectStatus>('NotStarted');
  const [startDate, setStartDate] = useState(toIsoDate(today));
  const [endDate, setEndDate] = useState(toIsoDate(new Date(today.getTime() + 1000 * 60 * 60 * 24 * 30)));
  const [saving, setSaving] = useState(false);

  // edit/delete state
  const [editingProject, setEditingProject] = useState<Project | null>(null);
  const [deletingProject, setDeletingProject] = useState<Project | null>(null);

  const load = async () => {
    if (!canRead) {
      setIsLoading(false);
      return;
    }
    setIsLoading(true);
    setError('');
    try {
      const data = await listProjects();
      setProjects(data);
    } catch {
      setError('Failed to load projects');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWrite) return;

    const user = getStoredUser();
    if (!user?.id) {
      setError('Missing current user. Please log in again.');
      return;
    }

    setSaving(true);
    setError('');
    try {
      await createProject({
        title,
        description,
        goal,
        status,
        startDate,
        endDate,
        managerId: user.id,
        teamLeadId: null
      });
      setTitle('');
      setDescription('');
      setGoal('');
      setStatus('NotStarted');
      await load();
    } catch {
      setError('Failed to create project (check permissions + managerId)');
    } finally {
      setSaving(false);
    }
  };

  const openEditDialog = (project: Project) => {
    setEditingProject(project);
    setTitle(project.title);
    setDescription(project.description || '');
    setGoal(project.goal || '');
    setStatus(project.status);
    setStartDate(project.startDate);
    setEndDate(project.endDate);
  };

  const closeEditDialog = () => {
    setEditingProject(null);
    setTitle('');
    setDescription('');
    setGoal('');
    setStatus('NotStarted');
  };

  const onUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWrite || !editingProject) return;

    setSaving(true);
    setError('');
    try {
      await updateProject(editingProject.id, {
        title,
        description,
        goal,
        status,
        startDate,
        endDate,
        teamLeadId: editingProject.teamLeadId
      });
      closeEditDialog();
      await load();
    } catch {
      setError('Failed to update project');
    } finally {
      setSaving(false);
    }
  };

  const onDelete = async () => {
    if (!deletingProject) return;

    setError('');
    try {
      await deleteProject(deletingProject.id);
      setDeletingProject(null);
      await load();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete project');
      setDeletingProject(null);
    }
  };

  if (!canRead) {
    return (
      <div className="min-h-screen bg-slate-50/50 p-8">
        <Card>
          <CardHeader>
            <CardTitle>Projects</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-slate-600">You do not have permission to view projects.</p>
            <div className="mt-4">
              <Link href="/dashboard">
                <Button variant="outline">Back to Dashboard</Button>
              </Link>
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
          <h1 className="text-3xl font-bold tracking-tight text-slate-900">Projects</h1>
          <p className="text-slate-500">All projects (global access by permission)</p>
        </div>
        <Link href="/dashboard">
          <Button variant="outline">Back</Button>
        </Link>
      </div>

      {error && (
        <div className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-600 ring-1 ring-red-200">
          {error}
        </div>
      )}

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle>Project List</CardTitle>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-900 border-t-transparent" />
              ) : projects.length === 0 ? (
                <p className="text-sm text-slate-500">No projects found.</p>
              ) : (
                <div className="space-y-3">
                  {projects.map(p => (
                    <div key={p.id} className="rounded-md border border-slate-200 bg-white p-4">
                      <div className="flex items-start justify-between gap-4">
                        <Link href={`/projects/${p.id}`} className="flex-1 hover:text-slate-900">
                          <div>
                            <div className="font-medium text-slate-900">{p.title}</div>
                            <div className="text-sm text-slate-500 line-clamp-2">{p.description}</div>
                          </div>
                        </Link>
                        <div className="flex items-center gap-2">
                          <span className="text-xs text-slate-500">{p.status}</span>
                          {canWrite && (
                            <>
                              <Button
                                size="icon"
                                variant="ghost"
                                onClick={(e) => {
                                  e.preventDefault();
                                  openEditDialog(p);
                                }}
                              >
                                <Pencil className="h-4 w-4" />
                              </Button>
                              <Button
                                size="icon"
                                variant="ghost"
                                onClick={(e) => {
                                  e.preventDefault();
                                  setDeletingProject(p);
                                }}
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
              <CardTitle>Create Project</CardTitle>
            </CardHeader>
            <CardContent>
              <form onSubmit={onCreate} className="space-y-3">
                <fieldset disabled={!canWrite || saving} className="space-y-3">
                  {!canWrite && (
                    <div className="rounded-md bg-yellow-50 p-3 text-sm text-yellow-800 ring-1 ring-yellow-200">
                      You do not have permission to create projects.
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
                  <div>
                    <label className="text-sm font-medium">Goal</label>
                    <Input value={goal} onChange={(e) => setGoal(e.target.value)} required />
                  </div>
                  <div>
                    <label className="text-sm font-medium">Status</label>
                    <select
                      className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                      value={status}
                      onChange={(e) => setStatus(e.target.value as ProjectStatus)}
                    >
                      <option value="NotStarted">NotStarted</option>
                      <option value="InProgress">InProgress</option>
                      <option value="Completed">Completed</option>
                      <option value="OnHold">OnHold</option>
                      <option value="Cancelled">Cancelled</option>
                    </select>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="text-sm font-medium">Start</label>
                      <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
                    </div>
                    <div>
                      <label className="text-sm font-medium">End</label>
                      <Input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
                    </div>
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

      {/* Edit Project Dialog */}
      <Dialog open={!!editingProject} onOpenChange={(open) => !open && closeEditDialog()}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Project</DialogTitle>
            <DialogClose onClose={closeEditDialog} />
          </DialogHeader>
          <form onSubmit={onUpdate}>
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
                <div>
                  <label className="text-sm font-medium">Goal</label>
                  <Input value={goal} onChange={(e) => setGoal(e.target.value)} required />
                </div>
                <div>
                  <label className="text-sm font-medium">Status</label>
                  <select
                    className="flex h-9 w-full rounded-md border border-slate-200 bg-transparent px-3 py-1 text-sm shadow-sm"
                    value={status}
                    onChange={(e) => setStatus(e.target.value as ProjectStatus)}
                  >
                    <option value="NotStarted">NotStarted</option>
                    <option value="InProgress">InProgress</option>
                    <option value="Completed">Completed</option>
                    <option value="OnHold">OnHold</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium">Start</label>
                    <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
                  </div>
                  <div>
                    <label className="text-sm font-medium">End</label>
                    <Input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
                  </div>
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
        open={!!deletingProject}
        onOpenChange={(open) => !open && setDeletingProject(null)}
        onConfirm={onDelete}
        title="Delete Project"
        description={`Are you sure you want to delete "${deletingProject?.title}"? This action cannot be undone.`}
        confirmText="Delete"
        variant="danger"
      />
    </div>
  );
}
