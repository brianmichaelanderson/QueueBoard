# Data model

This document captures the reduced single-tenant data model (Queue + Agent) for QueueBoard, the Mermaid ER diagram, and the minimal API surface and seed suggestions used by the app.

## ER diagram (Mermaid)

```mermaid
erDiagram
    QUEUE {
        guid Id PK
        string Name
        string Description
        bool IsActive
        rowversion RowVersion
        datetime CreatedAt
        datetime UpdatedAt
    }

    AGENT {
        guid Id PK
        string FirstName
        string LastName
        string Email
        bool IsActive
        rowversion RowVersion
        datetime CreatedAt
        datetime UpdatedAt
    }

    AGENT_QUEUE {
        guid AgentId PK FK
        guid QueueId PK FK
        bool IsPrimary
        int SkillLevel
        datetime AssignedAt
    }

    QUEUE ||--o{ AGENT_QUEUE : "has"
    AGENT ||--o{ AGENT_QUEUE : "assigned to"
```

## Entities (concise fields)

- **Queue**: `Id (GUID)`, `Name` (required, indexed), `Description`, `IsActive`, `RowVersion` (concurrency), `CreatedAt`, `UpdatedAt`
- **Agent**: `Id (GUID)`, `FirstName`, `LastName`, `Email` (required, unique), `IsActive`, `RowVersion`, `CreatedAt`, `UpdatedAt`
- **AgentQueue** (join): `AgentId`, `QueueId`, `IsPrimary` (bool), `SkillLevel` (int), `AssignedAt`

Notes:
- Single-tenant for MVP (no `TenantId`). Add `TenantId` later if multi-tenant needed.
- Use explicit join entity `AgentQueue` (not EF shadow many-to-many) to store assignment metadata.
- Add indexes on `Queue.Name` and `Agent.Email` for search performance.

## Minimal API surface

Queues
- `GET /queues?search=&page=&pageSize=` — list (search + paging)
- `GET /queues/{id}` — details (include assigned agents)
- `POST /queues` — create
- `PUT /queues/{id}` — update (use `RowVersion` for concurrency)
- `DELETE /queues/{id}` — delete (soft-delete via `IsActive` optional)

Agents
- `GET /agents?search=&page=&pageSize=` — list
- `GET /agents/{id}`
- `POST /agents`
- `PUT /agents/{id}`
- `DELETE /agents/{id}`

Assignments
- `POST /queues/{id}/assignments` — assign/unassign agents (accepts list of `AgentId` + metadata such as `IsPrimary`, `SkillLevel`)

## Seed suggestions (development)

- Seed 3 queues and 6 agents with deterministic GUIDs so frontend/integration tests have stable data.
- Seed a few `AgentQueue` rows demonstrating primary/non-primary assignments and varying `SkillLevel`.

## Best practices / implementation notes

- Keep DTOs separate from EF entities; map explicitly (AutoMapper or manual mapping).
- Project lists to lightweight DTOs to avoid loading unnecessary navigation props (`Select` into DTO).
- Use optimistic concurrency via `rowversion` on entities that are edited frequently.
- Add validation attributes to DTOs (e.g., `Name` required, `Email` required + email format).
- Consider soft-delete (`IsActive`) for simpler UX; physically delete only when necessary.

---

File: docs/data-model.md
