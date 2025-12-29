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

4. [x] Configure structured logging
    - Add structured logs (Serilog or configure Microsoft logging) and logging scopes.
       - [x] 4.1 Add Serilog NuGet packages to server/QueueBoard.Api.csproj (`Serilog.AspNetCore`, `Serilog.Sinks.Console`, optionally `Serilog.Sinks.File`, `Serilog.Enrichers.Environment`)
       - [x] 4.2 Configure Serilog host in `Program.cs` (call `builder.Host.UseSerilog()` and initialize `Log.Logger` with JSON console sink and basic enrichers)
          - [x] 4.2.1 Run `dotnet restore` / `dotnet build` (or container build) to validate package restore and build before runtime verification
       - [x] 4.3 Add Serilog settings to `appsettings.Development.json` (console JSON formatting, minimum levels, optional Seq/file sinks)
       - [x] 4.4 Update key controllers/services to use `ILogger<T>` where useful (minimal, targeted changes)
       - [x] 4.5 Verify structured logs during a container run (use `docker compose up` and inspect logs for JSON/enriched fields)
      - [x] 4.6 Persist container logs to file and enable rotation
         - [x] 4.6.1 Update Serilog config to add file sink with rolling interval
         - [x] 4.6.2 Update Docker Compose volume mounts to persist log files outside container
         - [x] 4.6.3 Verify log files are created and rotated as expected during container runs

5. [ ] Global exception middleware + ProblemDetails
    - Centralize error handling and return RFC7807 `ProblemDetails` for errors.
      - [x] 5.1 Design error contract and mapping rules
          - Decide canonical `ProblemDetails` fields (title, status, detail, instance, traceId) and any project-specific extensions (e.g., `errors` array).
          - Define mapping rules: validation → 400, not-found → 404, conflict → 409, unhandled → 500.
      - [x] 5.2 Implement correlation-id / trace propagation (recommended early)
          - Add middleware to ensure every request has a correlation id (header `X-Correlation-ID`) and surface it in logs and `ProblemDetails` responses. This should run before exception middleware so exceptions include the correlation id.
        - [x] 5.3 ProblemDetails factory
           - Add a custom `ProblemDetailsFactory` or extend the default to include `traceId`, environment-safe details, and a consistent `errors` shape for validation failures.
      - [x] 5.4 Implement global exception-handling middleware
          - Create `Middleware/ExceptionHandlingMiddleware` to catch exceptions, map to `ProblemDetails` via the factory, set `application/problem+json`, and return proper status codes.
          - Ensure middleware logs the exception with structured context (path, method, user, correlation id).
      - [x] 5.5 Exception → status mapping (part of middleware)
          - Implement explicit mappings for common exceptions (model validation, `KeyNotFoundException`/domain not-found, `DbUpdateException`→409, etc.).
      - [x] 5.6 Register and order middleware
          - Wire middleware into `Program.cs` in the correct order (register correlation-id middleware first, then exception middleware, consider `UseDeveloperExceptionPage` in Development only).
       - [ ] 5.7 Avoid leaking secrets / redaction
          - Strip or redact sensitive info from `ProblemDetails` in non-development environments; ensure stack traces are not returned in production.
       - [x] 5.8 Tests (TDD)
          - [x] Added unit tests for:
               [x] `ExceptionHandlingMiddleware` mapping rules and logging (fast, deterministic).
               [x] `CorrelationIdMiddlewareTests.cs` (ensures correlation id propagation and context items)
               [x] `ExceptionHandlingMiddlewareTests.cs` (maps exceptions → status codes and ProblemDetails shape)
               [x] `ProblemDetailsFactoryTests.cs` (ensures `traceId`/`timestamp` enrichment and validation payload shape)
               - Tests located under `server/QueueBoard.Api/Tests/Unit`
          - [ ] Integration tests using `WebApplicationFactory` to assert response shape, headers, and end-to-end logging behaviour.
       - [x] 5.9 Documentation
          - Update `docs/part1-plan.md` and README with examples of error responses, status codes, and headers. See `docs/error-handling.md` for canonical examples and rules.

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

