# QueueBoard

QueueBoard is a small, end-to-end reference application built to exercise the same stack, patterns, and workflows used at Buildable.

The purpose of this project is **technical alignment**, not domain completeness. The application intentionally uses a simple admin-style domain to demonstrate:

- ASP.NET Core Web API fundamentals
- EF Core with SQL Server and migrations
- An Angular 20 SPA using standalone components
- Explicit lazy-loaded feature routes
- Clean API ↔ frontend integration

---

## What QueueBoard Does

QueueBoard provides a minimal administrative interface for managing queues and agents.

The application supports:

- Viewing paginated, searchable lists of queues
- Creating and editing queues
- Assigning agents to queues
- Navigating between Admin and Agent areas via lazy-loaded routes

The domain is intentionally lightweight and exists only to support the technical goals of the project.

---

## Tech Stack

### Backend

- ASP.NET Core Web API
- EF Core
- SQL Server Express
- Swagger / OpenAPI

### Frontend

- Angular 20
- Standalone components (standalone-first APIs)
- Angular Router with lazy-loaded routes
- Reactive Forms
- HttpClient

### Tooling

- .NET SDK
- Node LTS
- Angular CLI
- SQL Server Express
- Git

---

## Angular Architecture (Standalone-First)

The frontend is built using Angular 20's standalone APIs. NgModules are intentionally avoided in favor of:

- Standalone components
- Feature-scoped route configuration files
- Explicit lazy loading via the router

Feature areas are structured to preserve clear boundaries and make lazy loading obvious.

### Frontend Folder Structure

```
client/
  src/
    app/
      app.component.ts
      app.config.ts
      app.routes.ts

      admin/
        admin.routes.ts
        admin-shell.component.ts
        queue-list.component.ts
        queue-edit.component.ts

      agent/
        agent.routes.ts
        agent-dashboard.component.ts

      shared/
        api/
          queue.service.ts
          agent.service.ts
        models/
          queue.model.ts
          agent.model.ts
        guards/
          auth.guard.ts
        interceptors/
          auth.interceptor.ts
```

### Key Points

- Each feature area (`admin`, `agent`) owns its own route configuration
- Feature areas are lazy-loaded via `loadChildren`
- Shared services, models, guards, and interceptors live under `shared`
- No `CoreModule` / `SharedModule` patterns are used

---

### Backend Folder Structure

```
server/
  QueueBoard.Api/
    Program.cs
    appsettings.json
    Controllers/
      QueuesController.cs
      AgentsController.cs
    DTOs/
      QueueDto.cs
      AgentDto.cs
    Entities/
      Queue.cs
      Agent.cs
    QueueBoardDbContext.cs
    Migrations/
    Services/
      QueueService.cs
    Tests/
      Integration/
        WebAppFactoryTests.cs
      Unit/
  docker/
    docker-compose.yml
    sqlserver/Dockerfile
```

### Key Points (backend)

- Single API project (`QueueBoard.Api`) holds controllers, DTOs, DbContext, entities, migrations and small services — keeps the layout minimal while preserving separation-of-concerns.
- Keep EF Core `Migrations/` inside the API project so `dotnet ef` runs naturally from that folder.
- Integration tests (use `WebApplicationFactory`) belong under `Tests/Integration` while small unit tests live under `Tests/Unit`.
- Provide a `docker/docker-compose.yml` that brings up a Dockerized SQL Server for local dev/CI; prefer Dockerized SQL Server for full runs and reserve the in-memory provider only for isolated unit tests.


## Routing & Lazy Loading

Application routing is defined using standalone route configuration.

### Root Routes

```
/admin/...  → Admin feature (lazy-loaded)
/agent/...  → Agent feature (lazy-loaded)
```

Lazy loading is implemented explicitly using route-level imports.

**Example:**

```typescript
{
  path: 'admin',
  loadChildren: () => import('./admin/admin.routes')
}
```

Navigating to a feature route triggers loading of that feature's bundle.

---

## Backend Overview

- Clear separation between EF Core entities and API DTOs
- Global exception handling with consistent error responses
- Request validation via DataAnnotations
- Structured logging
- Pagination and filtering on list endpoints
- EF Core migrations managed end-to-end
- Development-only seed data

### Example Endpoints

```
GET    /health
GET    /queues?search=abc&page=1&pageSize=25
POST   /queues
PUT    /queues/{id}
```

---

## Authentication

Authentication is intentionally simplified.

A stub JWT implementation is used to demonstrate:

- Protected API endpoints
- Attaching tokens via Angular HTTP interceptors (functional interceptor pattern)

**Note:** No real user management or credential handling is implemented.

---

## Frontend Data Flow

- Angular services encapsulate all API calls
- HttpClient is used with typed DTOs
- Reactive forms handle create/edit workflows
- Loading and error states are handled explicitly at the component level
- State management libraries (NgRx, etc.) are intentionally **not** used

---

## Testing

### Backend

- Unit tests for core logic
- One integration test using `WebApplicationFactory` validating:
  - Create → Fetch → Update workflow

### Frontend

- Basic service and/or component tests
- Focus on correctness rather than coverage

---

## What This Project Is (and Is Not)

### This project **is**:

- A focused, end-to-end stack exercise
- A reference implementation aligned with Buildable's tooling
- A demonstration of clean, explicit application structure

### This project is **not**:

- A production-ready system
- A full call-center or ACD implementation
- A showcase of advanced Angular features unrelated to the tutorial goals

---

## Running the Project

### Backend

```bash
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend

```bash
npm install
ng serve
```

## Data persistence (Docker)

- The SQL Server data is stored in the named Docker volume `queueboard_mssql-data`.
  The host mountpoint is typically `/var/lib/docker/volumes/queueboard_mssql-data/_data`.
- Stopping or recreating containers preserves data. To delete the database files and start
  fresh, remove the volume with `docker-compose down -v` or `docker volume rm queueboard_mssql-data`.


## Documentation

- **Data model & minimal API:** [docs/data-model.md](docs/data-model.md)
- **API reference (detailed):** [docs/api.md](docs/api.md)

---

## Notes

- Design decisions favor clarity, explicitness, and alignment with Buildable's stack over domain complexity or framework novelty
- Where tradeoffs were required, simpler and more explicit patterns were preferred