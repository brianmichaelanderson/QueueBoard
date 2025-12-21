#!/usr/bin/env bash
set -euo pipefail

export DOTNET_CLI_TELEMETRY_OPTOUT=1

mkdir -p /src/tmp
LOG=/src/tmp/queueboard_api.log

echo "Starting QueueBoard API... (logs -> $LOG)"
 # If DEFAULT_CONNECTION contains unevaluated placeholders (from .env), build it here
 if [ -n "${DEFAULT_CONNECTION:-}" ] && echo "$DEFAULT_CONNECTION" | grep -q '\${'; then
   export DEFAULT_CONNECTION="Server=${DB_HOST},${DB_PORT};User Id=${DB_USER};Password=${SA_PASSWORD};TrustServerCertificate=True;"
   echo "Built DEFAULT_CONNECTION from components"
 fi

 dotnet run --project server/QueueBoard.Api > "$LOG" 2>&1 &
API_PID=$!

trap 'echo "Stopping API..."; kill "$API_PID" 2>/dev/null || true' EXIT

echo "Waiting for API to be healthy (trying ports 5000 and 8080)..."
HEALTH_URL=''
for i in {1..60}; do
  if curl -sS http://localhost:5000/health >/dev/null 2>&1; then
    HEALTH_URL=http://localhost:5000
    echo "API healthy on $HEALTH_URL"
    break
  fi
  if curl -sS http://localhost:8080/health >/dev/null 2>&1; then
    HEALTH_URL=http://localhost:8080
    echo "API healthy on $HEALTH_URL"
    break
  fi
  sleep 1
done

if [ -z "$HEALTH_URL" ]; then
  echo "API failed to start. Dumping log:" >&2
  tail -n 200 "$LOG" >&2 || true
  exit 1
fi

echo "GET /queues (truncated)"
curl -sS "$HEALTH_URL/queues" | head -c 2000 || true
echo

echo "GET /agents (truncated)"
curl -sS "$HEALTH_URL/agents" | head -c 2000 || true
echo

echo "HTTP checks complete."
