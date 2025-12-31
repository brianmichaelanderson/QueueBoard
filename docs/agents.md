# Agents API Examples

This document provides quick curl examples and expected responses for the `agents` endpoints, including `ETag`/`If-Match` usage.

Quick endpoints

- `GET /agents` → 200 (paginated list)
- `GET /agents/{id}` → 200 or 404 (returns `ETag` header)
- `POST /agents` → 201 Created (returns `ETag` header)
- `PUT /agents/{id}` → 204 NoContent or 400/404/409
- `DELETE /agents/{id}` → 204 NoContent or 404/412

Create an agent

```bash
curl -i -X POST -H "Content-Type: application/json" \
  -d '{"firstName":"Alice","lastName":"Anderson","email":"alice.anderson@example.com","isActive":true}' \
  http://localhost:8080/agents
```

Successful response (truncated):

```
HTTP/1.1 201 Created
ETag: "<base64-token>"
Content-Type: application/json; charset=utf-8

{
  "id": "00000000-0000-0000-0000-000000000001",
  "firstName": "Alice",
  "lastName": "Anderson",
  "email": "alice.anderson@example.com",
  "isActive": true,
  "createdAt": "2025-12-30T12:00:00Z",
  "rowVersion": "<base64-token>"
}
```

Get an agent and use the returned `ETag` for updates

```bash
# fetch agent and save ETag
curl -i http://localhost:8080/agents/00000000-0000-0000-0000-000000000001

# use If-Match with the ETag value when updating or deleting
ETAG='"<base64-token>"'
curl -i -X PUT -H "Content-Type: application/json" -H "If-Match: $ETAG" \
  -d '{"firstName":"Alice","lastName":"Anderson","email":"alice.new@example.com","isActive":true,"rowVersion":"<base64-token>"}' \
  http://localhost:8080/agents/00000000-0000-0000-0000-000000000001
```

Delete with precondition

```bash
ETAG='"<base64-token>"'
curl -i -X DELETE -H "If-Match: $ETAG" http://localhost:8080/agents/00000000-0000-0000-0000-000000000001
```

Expected error responses

- `400 Bad Request` — invalid payload or missing required fields (`application/problem+json` with `errors`).
- `404 Not Found` — unknown `id`.
- `409 Conflict` or `412 Precondition Failed` — optimistic concurrency failure when provided `rowVersion`/`If-Match` does not match current state.

Notes

- The API accepts either an `If-Match` header or `rowVersion` in the request body for updates; header takes precedence when present.
- `rowVersion` values are base64-encoded 8-byte little-endian ticks derived from `UpdatedAt.UtcTicks`.
- See `docs/etag.md` for detailed `ETag` usage and `application/problem+json` examples.
