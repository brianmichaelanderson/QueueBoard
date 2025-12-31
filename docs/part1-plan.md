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

5. [x] Global exception middleware + ProblemDetails
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
       - [x] 5.7 Avoid leaking secrets / redaction
          - Strip or redact sensitive info from `ProblemDetails` in non-development environments; ensure stack traces are not returned in production.
       - [x] 5.8 Tests (TDD)
          - [x] 5.8.1 Unit tests
             - [x] `CorrelationIdMiddlewareTests.cs` — ensures correlation id propagation and context items
             - [x] `ExceptionHandlingMiddlewareTests.cs` — maps exceptions → status codes and ProblemDetails shape
             - [x] `ProblemDetailsFactoryTests.cs` — ensures `traceId`/`timestamp` enrichment and validation payload shape
             - [x] `CustomProblemDetailsFactoryTests.cs` — covers production redaction and development preservation of `detail`
             - Tests located under `server/QueueBoard.Api/Tests/Unit`
          - [x] 5.8.2 Integration tests (HTTP end-to-end)
             - [x] `ProjectionsTests.cs` — integration smoke tests that call the running API (http://localhost:8080 or `TEST_API_BASE_URL`) to validate DTO shapes and middleware behavior
             - Integration tests executed inside the SDK/container to avoid host NuGet/network issues and to exercise real DB/middleware
          - [ ] 5.8.3 (optional) In-process `WebApplicationFactory` integration tests
             - [ ] Add/restore `WebApplicationFactory`-style tests if you want faster, in-process integration tests (requires content-root/solution layout adjustments)
       - [x] 5.9 Documentation
          - Update `docs/part1-plan.md` and README with examples of error responses, status codes, and headers. See `docs/error-handling.md` for canonical examples and rules.

6. [ ] Request validation: Minimal, TDD-first plan (MVP):
       - [x] 6.1 Define validation strategy
          - [x] 6.1.1 Choose DataAnnotations on DTOs for the MVP; reserve `FluentValidation` for richer/complex rules later.
          - [x] 6.1.2 Decide which rules live on DTOs (required, ranges, lengths) vs domain validators (uniqueness, cross-field constraints).
       - [x] 6.2 Annotate DTOs
          - [x] 6.2.1 Add `[Required]`, `[StringLength]`, `[Range]` to `QueueDto` and `AgentDto` for fields the UI depends on (e.g., `Name` required, `MaxWait` > 0).  
            - Note: `QueueDto.Name` annotated and `CreateQueueDto` constructor parameter uses `[param: Required]`. Agent DTO annotations pending.
       - [x] 6.3 Unit tests (TDD) for DTO validation
          - [x] 6.3.1 Write fast MSTest unit tests that validate DTOs via `Validator.TryValidateObject(...)` and assert expected validation messages.
       - [ ] 6.4 Domain / cross-field validators (unit-tested)
          - [ ] 6.4.1 Implement small validator classes for rules that need logic (e.g., `MaxWait` ≤ SLA), test with unit tests and mocks for any service/DB dependencies.
       - [x] 6.5 Enforce model validation → `ValidationProblemDetails`
          - [x] 6.5.1 Ensure `[ApiController]` behavior is enabled so model binding failures produce `ValidationProblemDetails`.
          - [x] 6.5.2 Ensure `CustomProblemDetailsFactory` is used so `traceId`/`timestamp` and `errors` shape are consistent.
       - [x] 6.6 Integration tests (HTTP end-to-end)
          - [x] 6.6.1 Add integration tests that POST/PUT invalid DTOs → expect `400` `application/problem+json`, `errors` object, and `traceId` present (run inside SDK/container).
       - [x] 6.7 Docs & README updates
          - [x] 6.7.1 Add examples of invalid request → `ValidationProblemDetails` to `docs/error-handling.md` and reference commands for running validation tests.
       - [ ] 6.8 Optional: FluentValidation integration
          - [ ] 6.8.1 If richer rules are required later, add `FluentValidation.AspNetCore`, register validators, and map failures to `ValidationProblemDetails`.

7. [x] Implement Queues endpoints: List (search + pagination), Get by id, Create, Update, Delete. Use DTOs and projection queries.
   - 7.1 Decide delete semantics (hard-delete vs soft-delete)
      - [x] 7.1.1 Choose hard-delete or soft-delete for `Queue` and document rationale
      - [ ] 7.1.2 If soft-delete chosen, define `IsDeleted` semantics and retention/cleanup policy
   - 7.2 TDD: Controller & service unit tests (write tests first)
      - [x] 7.2.1 Write unit tests for `QueuesController.Delete` behaviors (204 on success, 404 when missing, idempotency)
      - [x] 7.2.2 Write unit tests for service/repository behaviors (delete persistence, soft-delete flag, exceptions mapping)
   - [x] 7.3 TDD: Integration tests (HTTP end-to-end)
   - [x] 7.3.1 Write integration test: create → delete → get (expect 404)
   - [x] 7.3.2 Write integration test: delete idempotency (repeat delete returns 204)
   - [x] 7.3.3 Write integration test: conflict/concurrency scenarios (if concurrency tokens/ETags used → 409)
   - [x] 7.4 Test infra & fixtures
      - [x] 7.4.1 Add/extend integration test fixtures to create isolated test data and reset DB between tests (unit/service tests use EF InMemory)
      - [x] 7.4.2 Add helper to poll `/health` and ensure API readiness before running integration tests
   - 7.5 API contract, status codes & docs
      - [x] 7.5.1 Define status code semantics for DELETE (204 on success, idempotent deletes return 204)
      - [x] 7.5.2 Add Swagger examples for DELETE and document in README/docs
      - [x] 7.5.3 Define ETag/If-Match semantics and emit `ETag` / accept `If-Match` on relevant endpoints
   - 7.6 Implementation (after tests exist and fail)
      - [x] 7.6.1 Implement `DELETE /queues/{id}` in `QueuesController` and call service/repo
      - [x] 7.6.2 Add/adjust service and repository logic (hard-delete implemented in `QueueService.DeleteAsync`)
      - [ ] 7.6.3 Add EF migration for soft-delete (if chosen)
      - [x] 7.6.4 Add logging/telemetry for delete events (include `traceId`, user, queueId)
   - 7.7 CI & smoke tests
      - [x] 7.7.1 Run containerized smoke tests (create→delete→fetch) in CI/local to validate end-to-end behavior
      - [x] 7.7.2 Ensure tests are deterministic and cleanup DB state after runs

Minimal remaining items to meet MVP intent for Task 7:
- Completed: integration tests (7.3.1, 7.3.2, 7.3.3) and readiness helper (7.4.2); idempotent delete behavior implemented.
- Completed: deterministic concurrency (ETag / RowVersion token) surfaced in DTOs and emitted as `ETag` headers; middleware maps concurrency → 409.
- Remaining: (none for 7.5) Swagger examples and README snippets for `DELETE /queues/{id}` completed.
- Remaining: Add CI smoke tests to run containerized create→delete→fetch and fail-fast on regressions (7.7.1).
- Remaining/Optional: Add logging/telemetry for delete events (7.6.4) and address XML doc warnings. 

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

