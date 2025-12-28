# Part 1 — ASP.NET Core Web API Plan

This document breaks Part 1 into numbered, high-level tasks to reference as we implement the API baseline.


1. [x] Verify dev environment
   - Confirm .NET SDK, Docker Compose, SQL Server container, and local runs behave as expected.

2. [x] Health endpoint
   - Add liveness/readiness probe (`GET /health`) that verifies app process and DB connectivity.

3. [x] Enable Swagger / OpenAPI
   - [x] 3.1 Add Swagger services in Program.cs
     - [x] 3.2 Configure SwaggerGen metadata and XML comments
        - [x] 3.2.1 Build to generate XML docs and verify Swagger picks them up
        - [x] 3.2.2 Add XML comments to controllers/DTOs to populate Swagger UI
   - [x] 3.3 Map Swagger UI in development only
   - [ ] 3.4 (optional) Add API versioning/grouping
   - [x] 3.5 Update README/docs with Swagger URL and usage

4. [ ] Configure structured logging
   - Add structured logs (Serilog or configure Microsoft logging) and logging scopes.
     - [ ] 4.1 Add Serilog NuGet packages to server/QueueBoard.Api.csproj (`Serilog.AspNetCore`, `Serilog.Sinks.Console`, optionally `Serilog.Sinks.File`, `Serilog.Enrichers.Environment`)
       - [ ] 4.2 Configure Serilog host in `Program.cs` (call `builder.Host.UseSerilog()` and initialize `Log.Logger` with JSON console sink and basic enrichers)
          - [ ] 4.2.1 Run `dotnet restore` / `dotnet build` (or container build) to validate package restore and build before runtime verification
     - [ ] 4.3 Add Serilog settings to `appsettings.Development.json` (console JSON formatting, minimum levels, optional Seq/file sinks)
     - [ ] 4.4 Update key controllers/services to use `ILogger<T>` where useful (minimal, targeted changes)
     - [ ] 4.5 Verify structured logs during a container run (use `docker compose up` and inspect logs for JSON/enriched fields)

5. [ ] Global exception middleware + ProblemDetails
   - Centralize error handling, return RFC7807 ProblemDetails for errors.

6. [ ] Request validation
   - Add DataAnnotation or FluentValidation rules and consistent validation error payloads.

7. [ ] Implement Queues endpoints
   - List (search + pagination), Get by id, Create, Update, Delete. Use DTOs and projection queries.

8. [ ] Implement Agents endpoints
   - CRUD endpoints for `Agent` with analogous patterns to `Queues`.

9. [ ] DTOs, mappings & EF configurations
   - Define DTOs, mapping strategies, and `IEntityTypeConfiguration<>` for each entity.

10. [ ] API conventions
    - Standardize pagination response shape, status code conventions, and idempotency behavior for POST/PUT.

11. [ ] Tests: integration + unit
    - Add a WebApplicationFactory integration test for create→fetch→update and basic unit tests for services.

12. [ ] Docs / README updates
    - Document endpoints, run instructions, and development notes (including `dotnet watch` behaviour).

---

How to mark a task done: change `[ ]` to `[x]` next to the numbered item.

