#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Resetting SQL Server database (drop + recreate) using EF Core migrations..."

# Load .env into the environment if present so TEST_DB_HOST/TEST_DEFAULT_CONNECTION are available
if [ -f .env ]; then
  while IFS='=' read -r key value || [ -n "$key" ]; do
    # skip comments and empty lines
    [[ "$key" =~ ^\s*# ]] && continue
    key=$(echo "$key" | sed 's/\s//g')
    if [ -z "$key" ]; then
      continue
    fi
    # remove optional surrounding quotes from value
    value="${value#\"}"
    value="${value%\"}"
    export "$key"="$value"
  done < .env
fi

# Decide target DB host/name from env (TEST_* take precedence)
TARGET_DB_HOST="${TEST_DB_HOST:-${DB_HOST:-db}}"
TARGET_DB_PORT="${DB_PORT:-1433}"

echo "Using target DB host: $TARGET_DB_HOST"

# Ensure target DB service is running
docker compose up -d "$TARGET_DB_HOST" || true

# Wait for DB TCP port to be available inside the target DB container
for i in {1..60}; do
  if docker compose exec -T "$TARGET_DB_HOST" bash -lc 'bash -c "echo > /dev/tcp/127.0.0.1/1433"' >/dev/null 2>&1; then
    echo "DB appears reachable"
    break
  fi
  echo "  waiting for DB to accept connections... ($i)"
  sleep 1
done

# Run EF Core commands from the api SDK container so we pick up project settings and env vars
# Ensure DB_HOST env is set to the compose service name so the API can resolve it
# Use --no-deps so we don't start a second DB when running the api container
echo "Dropping and recreating database via 'dotnet ef' in the 'api' service container..."

DOCKER_ENV_ARGS=(--no-deps)
# Always construct a DEFAULT_CONNECTION from parsed env components so placeholders are expanded
DEFAULT_CONN="Server=${TARGET_DB_HOST},${TARGET_DB_PORT};"
if [ -n "${TEST_DB_NAME:-}" ]; then
  DEFAULT_CONN+="Database=${TEST_DB_NAME};"
fi
DEFAULT_CONN+="User Id=${DB_USER:-sa};Password=${SA_PASSWORD:-};TrustServerCertificate=True;"

DOCKER_ENV_ARGS+=( -e DEFAULT_CONNECTION="${DEFAULT_CONN}" )

echo "Dropping and recreating database via 'dotnet ef' in the 'api' service container..."

docker compose run --rm "${DOCKER_ENV_ARGS[@]}" api bash -lc \
  "set -e; \
   dotnet restore server/QueueBoard.Api; \
   dotnet tool install --global dotnet-ef --version 8.* || true; \
   export PATH=\"$PATH:/root/.dotnet/tools\"; \
   dotnet ef database drop --project server/QueueBoard.Api --force || true; \
   dotnet ef database update --project server/QueueBoard.Api;"

echo "Database reset complete."

# Exit without bringing down containers; leave orchestration to caller
exit 0
