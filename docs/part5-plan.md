# Part 5 — Integration polish (High-level plan)

Purpose
- Provide a concise, high-level checklist to complete the integration polish described in `QueueBoard-outline.md` (Part 5). No implementation steps — just the top-level work items.

Goals
- Finish end-to-end concerns and add a small set of integration and unit tests to verify create → fetch → update flows.

Top-level tasks

- [ ] 1. Configure CORS for dev and production
- [ ] 2. Implement a simple JWT auth stub on the backend
- [ ] 3. Add an Angular auth interceptor to attach the stub token
- [ ] 4. Introduce DTO versioning (e.g., `v1` foldering) for API DTOs
- [ ] 5. Enforce consistent naming conventions across API/DB/TS
- [x] 6. Add backend unit tests (MSTest)
- [x] 7. Add backend integration tests using `WebApplicationFactory`
- [x] 8. Add frontend component/service tests (a couple shallow tests)
- [x] 9. Implement a happy-path integration test (create → fetch → update)
- [x] 10. Update docs and README to reflect changes and run instructions

Acceptance criteria
- CORS is configured appropriately for local dev and production scenarios.
- A minimal auth shape exists (stub JWT + interceptor) allowing guarded routes and guarded admin-only flows in the frontend.
- DTO versioning is in place and API surface is organized under a `v1` layout.
- Naming conventions are documented and applied to new code.
- Backend and frontend tests exist and a happy-path integration test validates create → fetch → update.

Notes
- Keep changes minimal and reversible; prefer feature flags/stubs for auth to avoid blocking other work.
- Integration tests should target an in-memory or test database via `WebApplicationFactory` and be runnable in CI.

Note: Documentation and README updates for routing/admin demos, zoneless testing guidance, and developer auth helpers have been applied (see `docs/part4-plan.md`, top-level `README.md`, and `client/README.md`). Frontend dev helpers (`AuthService.becomeAdmin()` / `becomeUser()`), admin wrapper routes, resolver wiring, and associated unit tests were added as part of Part 4 and referenced in these docs.

File: docs/part5-plan.md

