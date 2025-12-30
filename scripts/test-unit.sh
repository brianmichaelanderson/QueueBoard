#!/usr/bin/env bash
set -euo pipefail

# Run unit tests inside the api SDK container to avoid host NuGet/connectivity issues.
cd "$(dirname "${BASH_SOURCE[0]}")/.."

# Pass any extra args through to dotnet test
docker compose run --rm api bash -lc "cd /src/server/QueueBoard.Api && dotnet test --filter 'TestCategory!=Integration&FullyQualifiedName!~Integration' $*"
