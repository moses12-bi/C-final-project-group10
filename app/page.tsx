import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { LayoutDashboard, ShieldCheck, Users } from 'lucide-react';

export default function Home() {
  return (
    <main className="flex min-h-screen flex-col">
      <header className="flex h-16 items-center justify-between border-b border-slate-200 bg-white px-8">
        <div className="flex items-center gap-2">
          <LayoutDashboard className="h-6 w-6 text-slate-900" />
          <span className="text-xl font-bold tracking-tight text-slate-900">ProjectM</span>
        </div>
        <nav className="flex items-center gap-4">
          <Link href="/login">
            <Button variant="ghost">Sign In</Button>
          </Link>
          <Link href="/register">
            <Button>Get Started</Button>
          </Link>
        </nav>
      </header>

      <section className="flex flex-1 flex-col items-center justify-center bg-slate-50 px-4 py-24 text-center">
        <h1 className="text-4xl font-extrabold tracking-tight text-slate-900 sm:text-6xl">
          Manage Projects with <span className="text-blue-600">Precision</span>
        </h1>
        <p className="mt-6 max-w-2xl text-lg text-slate-600">
          The all-in-one platform for engineering teams to track performance, manage tasks,
          and collaborate seamlessly. Built for scale, designed for speed.
        </p>
        <div className="mt-10 flex gap-4">
          <Link href="/login">
            <Button size="lg" className="h-12 px-8 text-base">
              Go to Dashboard
            </Button>
          </Link>
          <Link href="https://github.com/your-repo">
            <Button variant="outline" size="lg" className="h-12 px-8 text-base">
              View Documentation
            </Button>
          </Link>
        </div>
      </section>

      <section className="grid gap-8 bg-white px-8 py-24 md:grid-cols-3">
        <div className="flex flex-col items-center text-center">
          <div className="mb-4 rounded-full bg-blue-50 p-4 ring-1 ring-blue-100">
            <LayoutDashboard className="h-8 w-8 text-blue-600" />
          </div>
          <h3 className="mb-2 text-xl font-bold text-slate-900">Project Tracking</h3>
          <p className="text-slate-600">Real-time updates on project milestones, budget utilization, and task completion rates.</p>
        </div>
        <div className="flex flex-col items-center text-center">
          <div className="mb-4 rounded-full bg-green-50 p-4 ring-1 ring-green-100">
            <Users className="h-8 w-8 text-green-600" />
          </div>
          <h3 className="mb-2 text-xl font-bold text-slate-900">Team Collaboration</h3>
          <p className="text-slate-600">Invite team members, assign tasks based on skills, and communicate in one place.</p>
        </div>
        <div className="flex flex-col items-center text-center">
          <div className="mb-4 rounded-full bg-purple-50 p-4 ring-1 ring-purple-100">
            <ShieldCheck className="h-8 w-8 text-purple-600" />
          </div>
          <h3 className="mb-2 text-xl font-bold text-slate-900">Secure Access</h3>
          <p className="text-slate-600">Enterprise-grade security with role-based access control and invitation-only registration.</p>
        </div>
      </section>

      <footer className="border-t border-slate-200 bg-white py-8 text-center text-sm text-slate-500">
        &copy; {new Date().getFullYear()} ProjectM Inc. All rights reserved.
      </footer>
    </main>
  );
}
