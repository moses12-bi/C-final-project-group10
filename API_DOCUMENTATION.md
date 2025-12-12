# Project Management Platform - API Documentation

## Base URL
```
Development: https://localhost:7XXX/api
Production: https://your-domain.com/api
```

## Authentication
All endpoints require JWT authentication via Bearer token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Common Response Codes
- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `204 No Content` - Request successful, no response body
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Missing or invalid authentication
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Endpoints

### Authentication
**POST /auth/login**
```json
Request:
{
  "email": "user@example.com",
  "password": "password123"
}

Response (200):
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "Admin"
}
```

### Projects
**GET /projects**
- List all projects
- Permissions: `projects.read`
- Query params: `page`, `pageSize`

**GET /projects/{id}**
- Get project by ID
- Permissions: `projects.read`

**POST /projects**
- Create new project
- Permissions: `projects.write`
```json
{
  "title": "New Project",
  "description": "Project description",
  "goal": "Project goals",
  "status": "InProgress",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "managerId": "guid",
  "teamLeadId": "guid"
}
```

**PUT /projects/{id}**
- Update project
- Permissions: `projects.write`

**DELETE /projects/{id}**
- Delete project
- Permissions: `projects.write`

### Tasks
**GET /projects/{projectId}/tasks**
- List tasks for project
- Permissions: `tasks.read`

**POST /projects/{projectId}/tasks**
- Create task
- Permissions: `tasks.write`
```json
{
  "title": "Task title",
  "description": "Task description",
  "priority": "High",
  "status": "ToDo",
  "startDate": "2024-01-01",
  "deadline": "2024-01-31",
  "estimatedHours": 8
}
```

### Task Assignments
**GET /tasks/{taskId}/assignments**
- List task assignees

**POST /tasks/{taskId}/assignments**
- Assign user to task
```json
{
  "userId": "guid",
  "isPrimaryAssignee": false
}
```

**DELETE /tasks/{taskId}/assignments/{userId}**
- Unassign user from task

### Comments
**GET /tasks/{taskId}/comments**
- List comments

**POST /tasks/{taskId}/comments**
- Add comment
```json
{
  "content": "Comment text"
}
```

### Attachments
**GET /tasks/{taskId}/attachments**
- List attachments

**POST /tasks/{taskId}/attachments**
- Upload file (multipart/form-data)

**GET /tasks/{taskId}/attachments/{id}/download**
- Download file

### Notifications
**GET /notifications**
- List notifications
- Query: `unreadOnly=true`

**PUT /notifications/{id}/read**
- Mark as read

**PUT /notifications/read-all**
- Mark all as read

### Reports & Analytics
**GET /reports/project-status/{projectId}**
- Project analytics

**GET /reports/team-performance**
- Team metrics
- Query: `startDate`, `endDate`

**GET /reports/task-analytics**
- Task statistics
- Query: `projectId`

### Search
**GET /search**
- Global search
- Query: `query`, `type` (projects|tasks|users)

## Error Response Format
```json
{
  "error": {
    "message": "Error description",
    "statusCode": 400
  }
}
```

## Rate Limiting
- 1000 requests per hour per user
- Exceeding limit returns `429 Too Many Requests`

## Pagination
Paginated endpoints return:
```json
{
  "items": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 20,
  "totalPages": 5,
  "hasPrevious": false,
  "hasNext": true
}
```
