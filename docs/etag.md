# ETag / If-Match usage

This API uses an optimistic-concurrency token exposed as an `ETag` response header (and present in the `rowVersion` property of the `QueueDto` responses).

Token format
- The `ETag` value is a base64-encoded 8-byte little-endian `long` representing `UpdatedAt.UtcTicks`.
- Example header: `ETag: "mLq3...base64...=="`

Client usage patterns

1) Safe read/update
- GET resource:
  - GET /queues/{id}
  - Response includes `ETag` header and `rowVersion` in the body.
- Update using `If-Match` header or include `RowVersion` in body:
  - PUT /queues/{id}
  - Header: `If-Match: "<etag-value>"`
  - Body: `{ name, description, isActive, rowVersion: "<etag-value>" }`
- On success: `204 NoContent` + new `ETag` header.
- If the resource changed since you fetched it: `409 Conflict` or `412 Precondition Failed` (if `If-Match` provided and mismatched).

2) Delete with precondition
- To prevent accidental deletes, include `If-Match` with the ETag read from GET.
- If the ETag doesn't match current resource, API returns `412 Precondition Failed` with `application/problem+json`.

Curl examples

Get resource and capture ETag:

```bash
curl -i http://localhost:8080/queues/<id>
# look for ETag header and the rowVersion in JSON body
```

Delete with If-Match:

```bash
ETAG='"<base64-token>"'
curl -i -X DELETE -H "If-Match: $ETAG" http://localhost:8080/queues/<id>
```

PUT with If-Match (body includes rowVersion):

```bash
ETAG='"<base64-token>"'
curl -i -X PUT -H "Content-Type: application/json" -H "If-Match: $ETAG" -d '{"name":"New","description":"x","isActive":true,"rowVersion":"<base64-token>"}' http://localhost:8080/queues/<id>
```

Notes
- The API accepts either `If-Match` header or `RowVersion` in the update body; header takes precedence.
- The API emits an `ETag` header on GET and POST, and on successful PUT.
