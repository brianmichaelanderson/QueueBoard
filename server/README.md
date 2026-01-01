# QueueBoard — Backend (server)

This document contains backend-specific setup, run, and test instructions for the `QueueBoard.Api` project.

Tech stack (backend)
- ASP.NET Core Web API
- EF Core
- SQL Server Express (Docker)
- Swagger / OpenAPI

Quick start (repo root)
```bash
# from repo root
dotnet restore
dotnet ef database update --project server/QueueBoard.Api
dotnet run --project server/QueueBoard.Api
```

Dev build note
- Prefer running the API inside the SDK/container using Docker Compose to avoid transient host NuGet issues. Example:

```bash
docker compose up -d --build api
docker compose exec api bash -lc 'cd /src/server/QueueBoard.Api && dotnet test'
```

Backend folder structure

```
server/
  QueueBoard.Api/
    Program.cs
    appsettings.json
    Controllers/
      QueuesController.cs
      AgentsController.cs
    DTOs/
      QueueDto.cs
      AgentDto.cs
    Entities/
      Queue.cs
      Agent.cs
    QueueBoardDbContext.cs
    Migrations/
    Services/
      QueueService.cs
    Tests/
      Integration/
      Unit/
```

Running backend tests (recommended inside SDK container)

```bash
docker run --rm --network queueboard_default \
  --platform linux/amd64 \
  -v "$(pwd)":/src -w /src --env-file .env \
  mcr.microsoft.com/dotnet/sdk:10.0 bash -lc "dotnet restore server/QueueBoard.Api && dotnet test server/QueueBoard.Api -v minimal"
```

Integration tests (example)

```bash
docker compose exec api bash -lc "dotnet test /src/server/QueueBoard.Api/QueueBoard.Api.csproj --filter Category!=Unit -v minimal"
```

Scripts and helpers
- `scripts/reset-db.sh` — reset dev DB
- `scripts/run_http_checks.sh` — quick HTTP checks against running API

Notes
- EF migrations live inside the API project so `dotnet ef` runs naturally from that folder.
- The API registers a custom `ProblemDetailsFactory` and global exception middleware; see `docs/error-handling.md` for details and examples.
