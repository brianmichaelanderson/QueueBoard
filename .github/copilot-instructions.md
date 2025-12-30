# Copilot Instructions — QueueBoard

Purpose
- Short orientation for AI coding agents working on QueueBoard: a Buildable-style reference app (ASP.NET Core 10 + EF Core backend, Angular 20 frontend). Start by reading the high-level docs and the outline.

Quick files to read
- [README.md](README.md): overall architecture, run commands, and examples.
- [QueueBoard-outline.md](QueueBoard-outline.md): phased implementation plan and priorities.

Big picture (what matters)
- Two main areas: backend (ASP.NET Core Web API + EF Core + SQL Server) and frontend (Angular 20 SPA using standalone components). The repo is a reference implementation focused on clarity and alignment with Buildable practices.
- Frontend patterns: standalone components, feature-owned `*.routes.ts` files, explicit lazy-loaded feature routes (no NgModules). Shared code lives under a `shared` feature (services, models, guards, interceptors).
- Backend patterns: clear separation between EF entities and DTOs, global exception handling, DataAnnotation validation, migrations via EF Core, dev-only seed data.

Developer workflows discovered (documented / discoverable)
- Backend run: `dotnet restore`, `dotnet ef database update`, `dotnet run` (see [README.md](README.md)).
	- Note: prefer building and running the backend inside the SDK/container using Docker Compose. Local `dotnet` builds/tests often fail during NuGet restore in some environments; use the `api` SDK container to run `dotnet build`/`dotnet test` to avoid unreliable local NuGet connectivity.
- Frontend run: `npm install`, `ng serve` (see [README.md](README.md)).
- EF migrations: use the standard EF Core commands (`Add-Migration`, `Update-Database`) against SQL Server Express; migrations are the source of truth for schema changes.
- Tests: README references unit tests and an integration test using `WebApplicationFactory`—inspect test projects for exact names before editing.

Project-specific conventions
- No NgModule pattern: prefer standalone components and feature-level route files (examples in README proposed folder layout).
- Feature boundaries: each feature (e.g., `admin`, `agent`) owns its routes and components; lazy loading is explicit via route-level `loadChildren` imports.
- DTO-first API surface: controllers return DTOs, not EF entities; prefer projection queries for list endpoints.
- Simple JWT stub for auth in dev: expect an interceptor on the Angular side and protected endpoints on the API (no full auth system).

Integration points and examples
- HTTP API endpoints (examples): `GET /queues?search=...&page=1&pageSize=25`, `POST /queues`, `PUT /queues/{id}`.
- Frontend ↔ API: Angular services using `HttpClient` wrap API calls; interceptors attach the stub JWT token.

How to proceed when changing code
- Backend model/schema changes: update entity + DTO, create EF migration (`Add-Migration <Name>`), run `dotnet ef database update`, and add/adjust seed data if needed.
- Frontend feature changes: follow the standalone-first pattern — create standalone components and a feature `*.routes.ts` that the root router lazy-loads.
- Tests: update or add unit tests that validate the new behavior; for integration, prefer `WebApplicationFactory`-based tests for end-to-end server scenarios.

Missing / unknown items you may need to ask about
- CI build commands, matrix, or secrets for SQL Server — not discoverable from current files.
- Any company-specific tooling or scripts (local dev DB init scripts, Docker compose, env var conventions).

If you modify this file
- Keep it concise. Update the "Quick files to read" and the run commands if you add CI or scripts.

Next actions for an AI agent
- Read [README.md](README.md) and [QueueBoard-outline.md](QueueBoard-outline.md) first.
- Locate actual project folders (search for `*.csproj`, `angular.json`, `package.json`) to confirm layout before making edits.
- Ask the repo owner for missing CI/build/test commands and the preferred branch/PR etiquette.

Confirmed assumptions
1. Frontend lives in `client/` (Angular 20, standalone components).
2. Backend expects EF Core migrations and a Dockerized SQL Server Express for full runs; use Dockerized SQL Server for local dev/CI and reserve the in-memory provider only for isolated unit tests.
3. Tests: expect a backend integration test using `WebApplicationFactory` plus basic frontend service/component tests.
4. Minimal deliverable: Queues + Agents CRUD (list, edit), search + pagination, validation + consistent error responses.
5. Auth: stub JWT only — provide an interceptor + guard demo; no full user management required.
6. Edit policy: do not modify repository docs or files without explicit permission from the repo owner.

---
Created/updated by an AI assistant to help new contributors and coding agents ramp faster. Please tell me what else you'd like included (CI, folder specifics, or code examples).
