# QueueBoard

QueueBoard is a small, end-to-end reference application that demonstrates an ASP.NET Core API and an Angular SPA.

Project MVP requirements were derived from [QueueBoard-outline.md](QueueBoard-outline.md).

See the platform-specific README files for detailed setup and run instructions:

- Server (backend): [server/README.md](server/README.md)
- Client (frontend): [client/README.md](client/README.md)

For developer guidance, architecture notes, and reference docs, see the `docs/` folder.

Quick links

- API conventions: [docs/api-conventions.md](docs/api-conventions.md)
- Error handling: [docs/error-handling.md](docs/error-handling.md)
- Data model: [docs/data-model.md](docs/data-model.md)

If you prefer a single-page overview, the original README content was split: backend-related instructions moved to `server/README.md` and frontend setup moved to `client/README.md`.
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
- Swagger / OpenAPI — see `docs/api-conventions.md` and the generated OpenAPI for request/response shapes and examples.

ETag / optimistic concurrency and other API contract conventions are summarized in `docs/api-conventions.md`. For protocol-level examples and curl snippets, see `docs/etag.md`, `docs/queues.md`, and `docs/agents.md`.

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

For frontend-specific documentation about the `API_BASE_URL` injection token (usage, TestBed override and bootstrap/provider examples), see the frontend README: [client/README.md](client/README.md#api-base-url-token-frontend).

For developer-focused instructions for the Agents feature (local dev, reset/seed, and focused frontend tests), see the Agents dev workflow in the frontend README: [client/README.md](client/README.md#agents-dev-workflow-mvp).

---

## Dev build note

Prefer building and running the backend inside the SDK/container using Docker Compose. Local `dotnet` builds and tests often fail during NuGet restore in some environments; running inside the `api` SDK container avoids unreliable local NuGet connectivity.

Example commands (run from the repository root):

```bash
docker compose up -d --build api
docker compose exec api bash -lc 'cd /src/server/QueueBoard.Api && dotnet test'
```

For queue- and agent-specific curl examples (create/get/update/delete) see `docs/queues.md` and `docs/agents.md`. For protocol-level details about ETag/If-Match and canonical error bodies see `docs/etag.md`.


## Angular Architecture (Standalone-First)

The frontend is built using Angular 20's standalone APIs. NgModules are intentionally avoided in favor of:

- Standalone components
- Feature-scoped route configuration files
- Explicit lazy loading via the router

Feature areas are structured to preserve clear boundaries and make lazy loading obvious.

### Key Points Frontend

- Each feature area (`admin`, `agent`) owns its own route configuration
- Feature areas are lazy-loaded via `loadChildren`
- Shared services, models, guards, and interceptors live under `shared`
- No `CoreModule` / `SharedModule` patterns are used

### Key Points Backend

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

Note: The API registers a custom `ProblemDetailsFactory` to enrich and redact RFC7807 responses (adds `traceId`/`timestamp` and strips `detail` in non-development environments). See `docs/error-handling.md` for examples and test guidance.

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

### Running with Docker Compose (recommended for local dev).  

Running `docker compose up --build -d` from the repo root will start both the `db` and `api` services defined in `docker-compose.yml` (the `api` service maps `${API_PORT:-8080}:8080` by default).

- Development note: the `api` service runs the API using `dotnet run` inside the SDK image by default (see the `command` in `docker-compose.yml`). This runs without hot-reload. To enable automatic reloads during development either run `dotnet watch run` inside the API project locally or update the `api` service `command` in `docker-compose.yml` to use `dotnet watch`.
  To run a production-style image instead, build a publish image (multi-stage Dockerfile) and run `docker compose up --build` or run the API locally with `dotnet run`.


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

Integration test approach
- The repository's integration tests are written to exercise the running API (HTTP) rather than always using an in-process TestHost. When executed inside the SDK container or via `docker compose exec api`, the tests call the API at `http://localhost:8080` (or at the `TEST_API_BASE_URL` environment variable if set).
- This avoids solution-root/content-root lookup issues when running tests inside containers and better reflects end-to-end behavior against the database and middleware.

Typical workflow (compose running):

```bash
docker compose up --build -d   # start db + api (api listens on 8080)
docker compose exec api bash -lc "dotnet test /src/server/QueueBoard.Api/QueueBoard.Api.csproj --filter Category!=Unit -v minimal"
```

Or run individual integration tests against the running service:

```bash
docker compose exec api bash -lc "dotnet test /src/server/QueueBoard.Api/QueueBoard.Api.csproj --filter FullyQualifiedName~ProjectionsTests --logger 'console;verbosity=normal' --no-restore"
```

- Unit tests (run an individual unit test or run the API project's tests inside the SDK container):

Run all tests in the API project (inside the SDK container):

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "dotnet restore server/QueueBoard.Api && dotnet test server/QueueBoard.Api/QueueBoard.Api.csproj -v minimal"
```

Run a single unit test by fully-qualified name (useful for fast feedback while developing middleware/tests):

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "dotnet test server/QueueBoard.Api/QueueBoard.Api.csproj --filter FullyQualifiedName~ProblemDetailsFactoryTests --logger 'console;verbosity=normal' --no-restore"
```

If you already have the `api` service running via `docker compose up`, you can run tests directly inside that container:

```bash
docker compose exec api bash -lc "dotnet test /src/server/QueueBoard.Api/QueueBoard.Api.csproj -v minimal"
```

Or run a single test by name inside the running `api` service:

```bash
docker compose exec api bash -lc "dotnet test /src/server/QueueBoard.Api/QueueBoard.Api.csproj --filter FullyQualifiedName~ProblemDetailsFactoryTests --logger 'console;verbosity=normal' --no-restore"
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

Additional test helpers
- `scripts/test-unit.sh` — runs the API project's unit tests inside the `api` SDK container and explicitly excludes integration tests. Use this for fast, local feedback.
- `scripts/test-integration.sh` — starts `db_test` and `api`, runs `./scripts/reset-db.sh` to reset the test DB, then runs only the integration tests inside the `api` container.

Examples:
```
./scripts/test-unit.sh
./scripts/test-integration.sh
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
- **API conventions & reference (detailed):** [docs/api-conventions.md](docs/api-conventions.md)

---

## Notes

- Design decisions favor clarity, explicitness, and alignment with Buildable's stack over domain complexity or framework novelty
- Where tradeoffs were required, simpler and more explicit patterns were preferred