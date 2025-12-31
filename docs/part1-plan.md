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

7. [x] Implement Queues endpoints
   - [x] 7.1 Design & API contract
      - [x] 7.1.1 Define DTOs: `QueueDto`, `CreateQueueDto`, `UpdateQueueDto` (Id, Name, optional settings, CreatedAt, UpdatedAt, `rowVersion` when using concurrency)
      - [x] 7.1.2 Define routes and status codes:
         - `GET /queues` → 200, supports `search`, `page`, `pageSize`
         - `GET /queues/{id}` → 200 or 404
         - `POST /queues` → 201 Created, returns `QueueDto` + `ETag`
         - `PUT /queues/{id}` → 204 NoContent, 400, 404, 409 (concurrency)
         - `DELETE /queues/{id}` → 204 NoContent, 404 (idempotent semantics)
      - [x] 7.1.3 Add XML comments and Swagger examples for each endpoint and DTO (align with project conventions)
   - [x] 7.2 TDD: Unit tests (fast, in-memory)
      - [x] 7.2.1 Controller unit tests (TDD-first): create failing tests for expected behaviors, then implement controllers to satisfy them
         - 7.2.1.1 `Create_ReturnsCreated_WithETag`
         - 7.2.1.2 `GetById_ReturnsDto_OrNotFound`
         - 7.2.1.3 `Update_WithStaleRowVersion_ReturnsConflict`
         - 7.2.1.4 `Delete_Idempotent_ReturnsNoContent`
      - [x] 7.2.2 Service/repository unit tests (in-memory): persistence, validation, concurrency handling, and exception-to-status mapping
   - [x] 7.3 Data model & EF Core
      - [x] 7.3.1 `Queue` entity + `AgentQueue` join already modeled and configured
      - [x] 7.3.2 `QueueConfiguration` present and registered in `OnModelCreating`
      - [x] 7.3.3 EF migrations applied for initial schema; add migration if soft-delete is adopted later
      - [x] 7.3.4 Seed minimal dev data for local runs (optional and already included in `SeedData`)
   - [x] 7.4 Implementation (make tests pass)
      - [x] 7.4.1 Implement `QueuesController` endpoints (`GET`, `POST`, `PUT`, `DELETE`) following projection/DTO patterns
      - [x] 7.4.2 Implement `IQueueService` / `QueueService` with create/read/update/delete, projection queries, and hard-delete semantics
      - [x] 7.4.3 Emit `ETag` on create/read and accept `If-Match` for updates/deletes when concurrency is used
      - [x] 7.4.4 Add structured logging for queue lifecycle events (include `traceId`, user, queueId)
   - [x] 7.5 Integration tests & readiness
      - [x] 7.5.1 Integration tests (SDK/container) for happy path: create → delete → get (expect 404)
      - [x] 7.5.2 Negative tests: delete idempotency, concurrency conflict (409), and validation errors (400)
      - [x] 7.5.3 Use `reset-db.sh` and readiness helper to ensure deterministic DB state between runs
   - [x] 7.6 Docs, Swagger & examples
      - [x] 7.6.1 Add Swagger examples and README snippets showing `ETag` / `If-Match` semantics for queues
      - [x] 7.6.2 Document status codes, error shapes, and idempotency expectations in `docs/*`
   - [ ] 7.7 Optional polish (post-MVP)
      - [ ] 7.7.1 Tweak search, filtering and pagination behavior for `GET /queues`
      - [ ] 7.7.2 Consider soft-delete / retention policy (if chosen, add migration and cleanup policy)
      - [x] 7.7.3 Audit/logging for queue create/update/delete events already added (include `traceId`)

8. [ ] Implement Agents endpoints
   - [ ] 8.1 Design & API contract
      - [x] 8.1.1 Define DTOs: `AgentDto`, `CreateAgentDto`, `UpdateAgentDto` (Id, Name, IsActive, CreatedAt, rowVersion if using concurrency)
      - [x] 8.1.2 Define routes and status codes:
         - `GET /agents` → 200, supports `search`, `page`, `pageSize`
         - `GET /agents/{id}` → 200 or 404
         - `POST /agents` → 201 Created, returns `AgentDto` + `ETag`
         - `PUT /agents/{id}` → 204 NoContent, 400, 404, 409 (concurrency)
         - `DELETE /agents/{id}` → 204 NoContent, 404 (idempotent semantics as chosen)
      - [ ] 8.1.3 Add XML comments and Swagger examples for each endpoint and DTO (align with Task 7 conventions)
   - [x] 8.2 TDD: Unit tests (fast, in-memory)
      - [x] 8.2.1 Controller unit tests (TDD-first): create failing tests for expected behaviors, then implement controllers to satisfy them
         - [x] 8.2.1.1 `Create_ReturnsCreated_WithETag`
         - [x] 8.2.1.2 `GetById_ReturnsDto_OrNotFound`
         - [x] 8.2.1.3 `Update_WithStaleRowVersion_ReturnsConflict`
         - [x] 8.2.1.4 `Delete_Idempotent_ReturnsNoContent`
      - [ ] 8.2.2 Service/repository unit tests (in-memory DB or mocks): persistence, validation, concurrency, error mapping
      - [ ] 8.2.3 Validation tests: DTO DataAnnotations and any cross-field validators (TDD-first)
   - [ ] 8.3 Data model & EF Core
      - [ ] 8.3.1 Add `Agent` entity and any join entities (e.g., `AgentQueue`) if needed per domain model
      - [ ] 8.3.2 Add `AgentConfiguration` implementing `IEntityTypeConfiguration<Agent>` and register in `OnModelCreating`
      - [ ] 8.3.3 Add EF migration (`Add-Migration AddAgent`) and test `dotnet ef database update` in the SDK/container
      - [ ] 8.3.4 Seed minimal dev data for local runs (optional)
   - [ ] 8.4 Implementation (make tests pass)
      - [ ] 8.4.1 Implement `AgentsController` with the same patterns used for `QueuesController` (projection, DTO mapping, ETag emission)
      - [ ] 8.4.2 Implement `IAgentService` and concrete service: create, get, update, delete, with logging and exception mapping
      - [ ] 8.4.3 Implement repository layer / DbContext usage and mapping helpers
      - [ ] 8.4.4 Emit `ETag` / support `If-Match` and deterministic `rowVersion` (if concurrency adopted)
   - [ ] 8.5 Integration tests & readiness
      - [ ] 8.5.1 Add integration tests (SDK/container) for the agent happy path: create → get → update → delete
      - [ ] 8.5.2 Add negative tests: validation errors (400), not-found (404), concurrency conflicts (409)
      - [ ] 8.5.3 Use existing `ReadinessHelper` and `reset-db.sh` to ensure deterministic DB state between runs
   - [ ] 8.6 Docs, Swagger & examples
      - [ ] 8.6.1 Add Swagger examples for agents endpoints and update `README.md` and `docs/*` with usage snippets (curl examples showing `ETag`/`If-Match` semantics)
      - [ ] 8.6.2 Document expected status codes and error shapes in `docs/error-handling.md`
   - [ ] 8.7 Optional polish (post-MVP)
      - [ ] 8.7.1 Search, filtering and pagination tuning for `GET /agents`
      - [ ] 8.7.2 Audit logging for agent create/update/delete events (include `traceId`)
      - [ ] 8.7.3 Authorization/guards: protect agent endpoints with roles or claims (if needed)

Notes / ordering guidance (TDD-first)
   - Follow this sequence for efficient TDD flow:
      1. 8.1 — design DTOs and API contract (small, explicit surface)
      2. 8.2 — write failing unit tests for controllers and services
      3. 8.3 — add minimal EF model and in-memory wiring to satisfy unit tests
      4. 8.4 — implement controllers/services/repositories to make unit tests pass
      5. 8.5 — add integration tests and run them in the SDK/container using `reset-db.sh`
      6. 8.6 — add Swagger examples and docs
   - Reference: `QueueBoard-outline.md` — keep Agents MVP minimal (Name, IsActive, optional relationships) and reuse established patterns from Task 7.

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

