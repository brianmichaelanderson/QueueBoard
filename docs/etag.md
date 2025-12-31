# ETag / If-Match usage

This API uses an optimistic-concurrency token exposed as an `ETag` response header (and present in the `rowVersion` property of the `QueueDto` responses).

Token format
- The `ETag` value is a base64-encoded 8-byte little-endian `long` representing `UpdatedAt.UtcTicks`.
- Example header: `ETag: "mLq3...base64...=="`

Client usage patterns

This document describes the canonical ETag/If-Match mechanics used across resources. For per-resource examples and curl snippets, see the resource docs (for example `docs/queues.md` and `docs/agents.md`).

1) Safe read/update (general)
- GET resource (e.g., GET /{resource}/{id})
  - Response includes `ETag` header and `rowVersion` in the body.
- Update using `If-Match` header or include `RowVersion` in body (e.g., PUT /{resource}/{id})
  - Header: `If-Match: "<etag-value>"`
  - Body: include the resource fields and `rowVersion: "<etag-value>"`
- On success: `204 NoContent` + new `ETag` header.
- If the resource changed since you fetched it: `409 Conflict` or `412 Precondition Failed` (if `If-Match` provided and mismatched).

2) Delete with precondition (general)
- To prevent accidental deletes, include `If-Match` with the ETag read from GET.
- If the ETag doesn't match current resource, API returns `412 Precondition Failed` with `application/problem+json`.

Expected responses (canonical)

- `204 NoContent` — resource deleted (idempotent). Response may include a new `ETag` header when applicable.
- `404 NotFound` — the resource with the specified id does not exist.
- `412 Precondition Failed` — the provided `If-Match` ETag does not match the current resource state. The response is `application/problem+json` with `type`, `title`, `status`, `detail`, `instance`, and `traceId`.

Example `412` response body (application/problem+json) — use in tests as a canonical shape:

```json
{
  "type": "https://example.com/probs/precondition-failed",
  "title": "Precondition Failed",
  "status": 412,
  "detail": "The provided ETag does not match the current resource state.",
  "instance": "/{resource}/{id}",
  "traceId": "|trace-id-example|"
}
```

Notes
- The API accepts either `If-Match` header or `RowVersion` in the update body; header takes precedence.
- The API emits an `ETag` header on GET and POST, and on successful PUT.
