# Error handling & ProblemDetails (dev-focused)

Purpose
- Canonical examples and minimal rules for error responses during development. Keep this doc as the single source of truth for tests and middleware behavior.

Content-Type
- All error responses should use `application/problem+json`.

Canonical JSON examples (development)

- Unhandled / 500
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "NullReferenceException: Object reference not set to an instance of an object.",
  "instance": "/queues/123",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "timestamp": "2025-12-29T08:29:05Z"
}
```

- Not Found / 404
```json
{
  "type": "https://example.com/probs/not-found",
  "title": "Resource not found.",
  "status": 404,
  "detail": "Queue with id '123' was not found.",
  "instance": "/queues/123",
  "traceId": "00-...",
  "timestamp": "2025-12-29T08:30:00Z"
}
```

- Validation / 400
```json
{
  "type": "https://example.com/probs/validation",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [ "The Name field is required." ],
    "MaxWait": [ "MaxWait must be a positive integer." ]
  },
  "instance": "/queues",
  "traceId": "00-...",
  "timestamp": "2025-12-29T08:31:00Z"
}
```

Mapping rules (MVP)
- Validation exceptions → 400 with an `errors` object (field → string[]).
- Domain not-found → 404 with helpful `detail`.
- Conflict (e.g., unique constraint) → 409 with `detail`.
- Unhandled exceptions → 500 with generic `title` and `traceId`.

Correlation / traceId
- Always include `traceId` in the response. Mirror it in the `X-Correlation-ID` response header when present.
- Use `Activity.Current` or a lightweight correlation-id middleware to populate `traceId`.

Redaction rules (dev-first)
- Development:
  - Responses may include `detail` with a short exception message to aid debugging.
  - Tests may assert presence of `detail` and `traceId`.
- Non-development (production-like):
  - Do not return stack traces or sensitive data.
  - For 500s return a generic message (e.g., "An unexpected error occurred.") and `traceId` only.
  - Implemented: the `CustomProblemDetailsFactory` redacts `detail` in non-development environments to avoid leaking sensitive information. Unit tests cover both behaviors (development preserves `detail`, production omits it).
- Always:
  - Never echo secrets (passwords, connection strings, tokens) in response bodies.
  - Validation errors may include field names and messages but must not echo sensitive values back.

Logging guidance
- Log full exception details (message, stack, inner exceptions) at Error level to Serilog sinks.
- Keep responses safe — logs are the place for full diagnostics.

Testing guidance
- Unit tests should assert:
  - `Content-Type` is `application/problem+json`.
  - `status`, `title`, and `traceId` exist.
  - Validation responses include `errors` with expected keys/messages.
- Integration tests should verify end-to-end behavior and that the `traceId` in response correlates with logs.

Unit test for redaction behavior
- `server/QueueBoard.Api/Tests/Unit/CustomProblemDetailsFactoryTests.cs`: [server/QueueBoard.Api/Tests/Unit/CustomProblemDetailsFactoryTests.cs](server/QueueBoard.Api/Tests/Unit/CustomProblemDetailsFactoryTests.cs)

Location & usage
- File: `docs/error-handling.md` (this file)
- Link from `docs/part1-plan.md` and README under the API/dev section.

Test files (unit)
- `server/QueueBoard.Api/Tests/Unit/CorrelationIdMiddlewareTests.cs`: [server/QueueBoard.Api/Tests/Unit/CorrelationIdMiddlewareTests.cs](server/QueueBoard.Api/Tests/Unit/CorrelationIdMiddlewareTests.cs)
- `server/QueueBoard.Api/Tests/Unit/ExceptionHandlingMiddlewareTests.cs`: [server/QueueBoard.Api/Tests/Unit/ExceptionHandlingMiddlewareTests.cs](server/QueueBoard.Api/Tests/Unit/ExceptionHandlingMiddlewareTests.cs)
- `server/QueueBoard.Api/Tests/Unit/ProblemDetailsFactoryTests.cs`: [server/QueueBoard.Api/Tests/Unit/ProblemDetailsFactoryTests.cs](server/QueueBoard.Api/Tests/Unit/ProblemDetailsFactoryTests.cs)

Change notes
- Keep examples small and stable; update the doc if the contract changes. Tests should reference this doc as the canonical behavior.
