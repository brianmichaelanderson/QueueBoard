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
  docker-compose.yml (repo root)
    sqlserver/Dockerfile
```

### Key Points (backend)

- Single API project (`QueueBoard.Api`) holds controllers, DTOs, DbContext, entities, migrations and small services — keeps the layout minimal while preserving separation-of-concerns.
- Keep EF Core `Migrations/` inside the API project so `dotnet ef` runs naturally from that folder.
- Integration tests (use `WebApplicationFactory`) belong under `Tests/Integration` while small unit tests live under `Tests/Unit`.
-- Provide a `docker-compose.yml` at the repository root that brings up a Dockerized SQL Server for local dev/CI; prefer Dockerized SQL Server for full runs and reserve the in-memory provider only for isolated unit tests.


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

### Running with Docker Compose (recommended for local dev)

Running `docker compose up --build -d` from the repo root will start both the `db` and `api` services defined in `docker-compose.yml` (the `api` service maps `${API_PORT:-8080}:8080` by default).


## Data persistence (Docker)

- The SQL Server data is stored in the named Docker volume `mssql-data` (declared in `docker-compose.yml`).
  The host mountpoint is typically `/var/lib/docker/volumes/<project>_mssql-data/_data`.
- Stopping or recreating containers preserves data. To delete the database files and start
  fresh, remove the volume with `docker compose down -v` or `docker volume rm <project>_mssql-data`.

## Running tests / quick checks

- Quick (recommended) — run the HTTP checks script inside the .NET SDK container. This starts the API (so it can access the compose DB), waits for health, and calls `/queues` and `/agents`:

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "bash /src/scripts/run_http_checks.sh"
```

- Integration tests (runs the test project inside the SDK container — avoids host NuGet/network issues):

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "dotnet restore server/QueueBoard.Api && dotnet test server/QueueBoard.Api -v minimal"
```

- Notes:
  - The repository includes a lightweight script at `scripts/run_http_checks.sh` used for fast verification of migrations/seed and DTO projections.
  - Some macOS hosts may experience transient NuGet/vulnerability-metadata errors with `dotnet restore`; using the SDK container avoids that class of issues.
  - Ensure `.env` is present and the DB service from `docker-compose.yml` is running (so the API can connect) when running either command.

## Scripts

- `scripts/run_http_checks.sh` — quick verification helper. It starts the API (so it can reach the compose DB), waits for health, calls `/queues` and `/agents`, prints truncated responses, then stops the API.

Usage (preferred, runs inside the SDK container):

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "bash /src/scripts/run_http_checks.sh"
```

Or run directly on a machine with the .NET SDK available (ensure `.env` and DB are reachable):

```bash
chmod +x scripts/run_http_checks.sh
./scripts/run_http_checks.sh
```

## AsNoTracking guidance

- **What it does:** `.AsNoTracking()` tells EF Core not to track returned entities in the change tracker. This reduces memory usage and slightly improves query performance for read-only scenarios.
- **When to use:** apply `.AsNoTracking()` on read/list endpoints (especially when projecting to DTOs) where you do not intend to update the returned entities in the same DbContext scope. It is recommended for paging/search endpoints that return lists.
- **When not to use:** do not use `.AsNoTracking()` if you plan to modify the returned entities and save changes within the same `DbContext` instance — tracking is required to detect and persist updates.
- **Example (recommended pattern for list endpoints):**

```csharp
var items = await _db.Queues
  .AsNoTracking()
  .Where(q => q.Name.Contains(search))
  .OrderBy(q => q.Name)
  .Select(q => new QueueDto(q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt))
  .ToListAsync();
```

Applying `.AsNoTracking()` together with projection (`Select(...)`) gives the best performance for read endpoints because EF does not construct tracked entity instances and the query returns only the fields needed by the DTO.





## Documentation

- **Data model & minimal API:** [docs/data-model.md](docs/data-model.md)
- **API reference (detailed):** [docs/api.md](docs/api.md)

---

## Notes

- Design decisions favor clarity, explicitness, and alignment with Buildable's stack over domain complexity or framework novelty
- Where tradeoffs were required, simpler and more explicit patterns were preferred