# QueueBoard — Project Outline

## Overview

A compact, hands-on full-stack app that covers a typical Buildable-style stack: ASP.NET Core Web API, EF Core (SQL Server Express), and an Angular SPA with lazy routing — focused on practical implementation rather than theory.

## Goal (Deliverable)

Build "QueueBoard": a simple ACD-like app with CRUD and search.

- **Entities:** Tenant, Queue, Agent, Call (or reduce scope to Queue + Agent)
- **API:** CRUD endpoints + filtered search + pagination
- **Angular:** list/detail pages, create/edit forms
- **Routing:** lazy-loaded modules (AdminModule, AgentModule)
- **Polish:** validation, consistent error responses, logging, Swagger, CORS

---

## Part 0 — Tooling setup (half day)

1. Install & verify
	- .NET SDK (match your internal version)
	- SQL Server Express + SSMS (or Azure Data Studio)
	- Node LTS + Angular CLI
	- Git + a diff tool
2. Quick smoke tests
	- `dotnet new webapi` builds and runs
	- SQL Express instance is reachable
	- `ng new` builds and serves

---

## Part 1 — ASP.NET Core Web API fundamentals (1–2 days)

### Goals
- Create a clean API baseline with idiomatic ASP.NET Core patterns.

### Implement
- Controllers, routing, dependency injection, configuration (`appsettings*.json`, env vars)
- Swagger / OpenAPI
- Structured logging
- Request validation (DataAnnotations or FluentValidation)
- Global exception middleware + consistent problem details
- API conventions: status codes, idempotency, pagination shape

- See the detailed plan: [docs/part1-plan.md](docs/part1-plan.md).

### Checkpoint
- `GET /health`
- `GET /queues?search=...&page=1&pageSize=25`
- `POST /queues` with validation and proper error payloads

---

## Part 2 — EF Core + SQL Server Express (2 days)

### Goals
- Model the domain, add persistence, and deliver reliable migrations.

### Implement
- Create entities and relationships
- Configure `DbContext` and use `IEntityTypeConfiguration<>`
- Migrations end-to-end (`Add-Migration`, `Update-Database`)
- LINQ queries with `Include`/projection to DTOs
- Understand tracking vs `AsNoTracking`
- Add a concurrency token (e.g., `rowversion`) on at least one table
- Seed dev-only data

### Performance basics
- Avoid N+1 queries; prefer projection for list endpoints
- Index columns used for search/filtering

### Checkpoint
- One migration creates the schema cleanly
- Search endpoint returns DTOs efficiently (not EF entities)

---

## Part 3 — Angular SPA core (2 days)

### Goals
- Build a maintainable Angular app structure and core UI features.

### Implement
- Feature folders + shared UI
- Services for API calls (`HttpClient`)
- Reactive forms + validation
- Error and loading states
- Environment config + API base URL
- Dev proxy config to avoid CORS during local dev

- See the detailed plan: [docs/part3-plan.md](docs/part3-plan.md).

### Checkpoint
- `QueuesListComponent` with search + paging
- `QueueEditComponent` as a reactive form for create/update

---

## Part 4 — Angular routing + lazy loading (1 day)

### Goals
- Make lazy loading explicit and test the route-level UX.

### Implement
- App routes that lazy-load modules:
  - `/agent/...` → lazy-load `AgentModule`
  - `/admin/...` → lazy-load `AdminModule`
- Route guards (a simple auth guard is fine)
- Route resolvers or a "fetch-on-enter" pattern
- Optional: `PreloadingStrategy` (e.g., preload `AgentModule` but not `AdminModule`)

- See the detailed plan: [docs/part4-plan.md](docs/part4-plan.md).

### Checkpoint
- `ng build` produces separate lazy chunks
- Navigating to `/admin` loads the admin bundle only when needed

---

## Part 5 — Integration polish (1–2 days)

### Goals
- Finish end-to-end concerns and add basic tests.

### Implement
- CORS configuration for dev and prod
- Authentication shape (simple JWT stub is fine): interceptors + attach token
- DTO versioning strategy (e.g., `v1` foldering)
- Consistent naming conventions across API/DB/TS

### Testing
- Backend: MSTest unit tests + 1–2 integration tests (use `WebApplicationFactory`)
- Frontend: a couple component or service tests

### Checkpoint
- A "happy path" integration test proves create → fetch → update works

---

## Recommended order (if short on time)

1. Build API endpoints (in-memory first)
2. Add EF Core + migrations + SQL Express
3. Build Angular list + edit pages
4. Add lazy modules + guards
5. Add integration polish + tests

---

## Must-know topics (day 1 expectations)

- **ASP.NET Core:** DI lifetimes, configuration, middleware pipeline
- **DTOs vs entities:** validation patterns, async/await usage
- **EF Core + SQL Server:** migrations, relationships, projections, query performance
- **Angular:** `HttpClient` + RxJS basics, reactive forms, router + lazy modules
