#!/usr/bin/env bash
set -euo pipefail

# Start test DB + API, reset DB, then run Integration tests inside the api SDK container.
cd "$(dirname "${BASH_SOURCE[0]}")/.."

docker compose up -d db_test api
./scripts/reset-db.sh

# Run integration tests inside the api container so restores/builds happen within the SDK container
docker compose run --rm api bash -lc "cd /src/server/QueueBoard.Api && TEST_API_BASE_URL=http://api:8080 dotnet test --filter 'TestCategory=Integration' $*"
