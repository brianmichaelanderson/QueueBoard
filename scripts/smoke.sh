#!/usr/bin/env bash
set -euo pipefail

# Minimal smoke test: create -> delete -> get(404)
# Designed to be run from the repository root.

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Starting smoke test (create -> delete -> fetch)..."

# Start services (api depends on db). For smoke runs prefer the test DB service.
docker compose up -d --build db_test api || docker compose up -d --build api

# Reset test DB to a clean state (uses TEST_DB_* env vars from .env)
./scripts/reset-db.sh

echo "Waiting for API health on http://localhost:8080/health"
status=000
for i in {1..60}; do
  status=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8080/health || echo "000")
  if [ "$status" = "200" ]; then
    echo "API healthy"
    break
  fi
  echo "  waiting... ($i)"
  sleep 1
done

if [ "$status" != "200" ]; then
  echo "API failed to become healthy (status=$status)"
  docker compose logs api --no-color | tail -n 200
  exit 2
fi

# Create queue and capture headers + body so we can read ETag directly from response headers
echo "Creating queue..."
TMP_DIR=$(mktemp -d)
POST_BODY_FILE="$TMP_DIR/body.json"
POST_HDR_FILE="$TMP_DIR/headers.txt"
curl -s -D "$POST_HDR_FILE" -H "Content-Type: application/json" -d '{"name":"smoke-queue","description":"smoke","isActive":true}' -o "$POST_BODY_FILE" -w "%{http_code}" http://localhost:8080/queues > "$TMP_DIR/code.txt"
code=$(cat "$TMP_DIR/code.txt" || echo "")
body=$(cat "$POST_BODY_FILE" || echo "")
if [ "$code" != "201" ]; then
  echo "Create failed: $code\n$body"
  cat "$POST_HDR_FILE" || true
  rm -rf "$TMP_DIR"
  exit 3
fi

# Extract id from response JSON (use python if available)
id=$(echo "$body" | python3 -c 'import sys,json; print(json.load(sys.stdin).get("id",""))')
if [ -z "$id" ] || [ "$id" = "null" ]; then
  echo "Failed to obtain id from create response"
  echo "$body"
  exit 4
fi
echo "Created queue id: $id"


# Prefer ETag from POST response headers; fallback to JSON body.rowVersion or GET if missing
etag=$(awk -F': ' '/^[Ee]tag:/ {gsub(/"/,"",$2); print $2; exit}' "$POST_HDR_FILE" || true)
if [ -z "$etag" ]; then
  # Try to read RowVersion from the POST response body (JSON). This is more reliable in some envs.
  echo "ETag not present on POST response headers; checking POST body for rowVersion..."
  if command -v python3 >/dev/null 2>&1; then
    rv=$(python3 - <<PY
import sys, json
try:
    o = json.load(open('$POST_BODY_FILE'))
except Exception:
    sys.exit(2)
for k in ('rowVersion','RowVersion','rowversion'):
    v = o.get(k)
    if v:
        print(v)
        sys.exit(0)
sys.exit(1)
PY
)
    if [ $? -eq 0 ] && [ -n "$rv" ]; then
      etag="$rv"
    fi
  fi
fi
if [ -z "$etag" ]; then
  echo "ETag not present in POST body; fetching via GET..."
  etag=$(curl -sI http://localhost:8080/queues/$id | tr -d '\r' | awk -F': ' '/[Ee]tag/ {gsub(/"/,"",$2); print $2; exit}')
fi
if [ -z "$etag" ]; then
  echo "ETag not found in response headers"
  docker compose logs api --no-color | tail -n 200
  rm -rf "$TMP_DIR"
  exit 5
fi
echo "ETag: $etag"

echo "Deleting with If-Match..."
del_code=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE -H "If-Match: \"$etag\"" http://localhost:8080/queues/$id || echo "000")
if [ "$del_code" != "204" ]; then
  echo "Delete failed (expected 204): $del_code"
  docker compose logs api --no-color | tail -n 200
  exit 6
fi

echo "Verifying deletion (GET should return 404)..."
get_code=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8080/queues/$id || echo "000")
if [ "$get_code" = "404" ]; then
  echo "Smoke test succeeded"
  exit 0
else
  echo "After delete, GET returned: $get_code"
  exit 7
fi
