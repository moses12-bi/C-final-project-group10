# Project Management Platform - Frontend Setup

## Prerequisites
- Node.js 18+ and npm
- Backend API running

## Installation

```bash
cd ProjectFrontend
npm install
```

## Development

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000)

## Environment Variables

Create `.env.local`:
```env
NEXT_PUBLIC_API_URL=https://localhost:7XXX
```

## Project Structure

```
ProjectFrontend/
├── app/                    # Next.js app router pages
│   ├── analytics/         # Analytics dashboard
│   ├── login/            # Login page
│   ├── projects/         # Projects pages
│   │   └── [id]/        # Project detail
│   └── tasks/           # Tasks pages
│       └── [id]/        # Task detail
├── components/           # React components
│   ├── notifications/   # Notification components
│   ├── search/         # Search components
│   └── ui/             # Reusable UI components
├── lib/                # Utilities
│   ├── permissions.ts  # Permission checking
│   └── session.ts      # Session management
└── services/           # API service layer
    ├── api.ts         # Axios instance
    ├── projects.ts    # Projects API
    ├── tasks.ts       # Tasks API
    ├── comments.ts    # Comments API
    ├── attachments.ts # Attachments API
    ├── assignments.ts # Assignments API
    ├── teams.ts       # Teams API
    ├── users.ts       # Users API
    ├── notifications.ts # Notifications API
    └── analytics.ts   # Analytics/Reports API
```

## Key Features

- ✅ JWT Authentication
- ✅ Permission-based UI rendering
- ✅ CRUD operations for Projects & Tasks
- ✅ Task assignments and team management
- ✅ Comments and file attachments
- ✅ Real-time notifications
- ✅ Analytics dashboard with charts
- ✅ Global search
- ✅ Loading skeletons
- ✅ Toast notifications
- ✅ Error boundaries
- ✅ Responsive design

## Building for Production

```bash
npm run build
npm run start
```

## Linting

```bash
npm run lint
```

## Technologies

- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **HTTP Client**: Axios
- **Charts**: Recharts
- **Icons**: Lucide React
- **UI Components**: Custom + Radix UI

## Common Tasks

### Adding a New Page
1. Create file in `app/your-page/page.tsx`
2. Add the component logic
3. Update navigation if needed

### Adding a New Service
1. Create file in `services/your-service.ts`
2. Export types and functions
3. Use in components via `import`

### Adding a New Component
1. Create in `components/ui/your-component.tsx`
2. Export from the file
3. Use via `import { YourComponent } from '@/components/ui/your-component'`
