# Queues API Examples

This document provides quick curl examples and expected responses for the `queues` endpoints, including `ETag`/`If-Match` usage.

Quick endpoints

- `GET /queues` → 200 (paginated list)
- `GET /queues/{id}` → 200 or 404 (returns `ETag` header)
- `POST /queues` → 201 Created (returns `ETag` header)
- `PUT /queues/{id}` → 204 NoContent or 400/404/409
- `DELETE /queues/{id}` → 204 NoContent or 404/412

Create a queue

```bash
curl -i -X POST -H "Content-Type: application/json" \
  -d '{"name":"Support","description":"Customer support queue","isActive":true}' \
  http://localhost:8080/queues
```

Successful response (truncated):

```
HTTP/1.1 201 Created
ETag: "<base64-token>"
Content-Type: application/json; charset=utf-8

{
  "id": "11111111-1111-1111-1111-111111111111",
  "name": "Support",
  "description": "Customer support queue",
  "isActive": true,
  "createdAt": "2025-12-30T12:00:00Z",
  "rowVersion": "<base64-token>"
}
```


Get a queue and use the returned `ETag` for updates

```bash
# fetch queue and inspect headers (look for ETag)
curl -i http://localhost:8080/queues/11111111-1111-1111-1111-111111111111

# Example: use If-Match with the ETag value when updating or deleting
ETAG='"<base64-token>"'
curl -i -X PUT -H "Content-Type: application/json" -H "If-Match: $ETAG" \
  -d '{"name":"Support","description":"Updated","isActive":true,"rowVersion":"<base64-token>"}' \
  http://localhost:8080/queues/11111111-1111-1111-1111-111111111111
```

Delete with precondition

```bash
ETAG='"<base64-token>"'
curl -i -X DELETE -H "If-Match: $ETAG" http://localhost:8080/queues/11111111-1111-1111-1111-111111111111
```

Expected error responses

- `400 Bad Request` — invalid payload or missing required fields (`application/problem+json` with `errors`).
- `404 Not Found` — unknown `id`.
- `412 Precondition Failed` — optimistic concurrency failure when provided `rowVersion`/`If-Match` does not match current state.

Notes

- The API accepts either an `If-Match` header or `rowVersion` in the request body for updates; header takes precedence when present.
- `rowVersion` values are base64-encoded 8-byte little-endian ticks derived from `UpdatedAt.UtcTicks`.
- See `docs/etag.md` for protocol-level details and canonical `application/problem+json` examples.
