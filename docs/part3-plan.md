# Part 3 — Angular SPA Plan (High-level)

Purpose
- High-level tasks to implement the Angular SPA core described in `QueueBoard-outline.md` (Part 3). This is an MVP-focused checklist — no deep substeps or implementation details here.

Goals
- Provide a maintainable standalone-component-based Angular app with list/detail pages, create/edit forms, and clear API integration for queues and agents.

Top-level tasks

1. [ ] Project setup
    - 1.1 [ ] Verify workspace presence
       - Check whether an Angular workspace already exists under `client/` or `frontend/` and decide whether to reuse or create a new workspace.
    - 1.2 [ ] Initialize the Angular workspace
       - Run `ng new <app-name>` (or `npm init` + manual setup) with standalone-component support and routing enabled.
    - 1.3 [ ] Install runtime and dev dependencies
       - Add Angular core packages plus dev tooling: `@angular/cli`, `@angular/core`, `@angular/compiler-cli`, `rxjs`, `zone.js` and dev deps like `typescript`, `eslint`, `prettier`.
    - 1.4 [ ] Add linting & formatting
       - Configure `eslint` (recommended) and `prettier`; add a shared config file (e.g., `.eslintrc.cjs`, `.prettierrc`) and `lint`/`format` scripts.
    - 1.5 [ ] Developer hooks and quality helpers (optional but recommended)
       - Add `husky` + `lint-staged` to run linters/formatters on pre-commit, and a basic `format` script that runs `prettier --write`.
    - 1.6 [ ] Add npm script set for local dev
       - Provide standard scripts in `package.json`: `start`, `build`, `serve`, `test`, `lint`, `format`, and `e2e` (if applicable).
    - 1.7 [ ] Local dev convenience and CI hints
       - Add a `proxy.conf.json` example and `README` snippets showing `npm install` and `npm run start` commands; include recommended CI commands (lint/test/build).

2. [ ] App shell & routing
   - Implement the root app shell and global routing. Configure lazy-loaded feature routes for `admin` and `agent` feature areas.

3. [ ] Feature scaffolding
   - Create feature folders for `admin` and `agent` with standalone components and a feature-level `*.routes.ts` file per feature.

4. [ ] Queues UI
   - Implement `QueuesListComponent` with search and paging inputs and `QueueEditComponent` (create/edit) with a reactive form.

5. [ ] Agents UI
   - Implement `AgentsListComponent` and `AgentDetail`/edit forms as needed for MVP.

6. [ ] API services
   - Add typed Angular services (`QueueService`, `AgentService`) using `HttpClient` to call the API and centralize request/response handling (including `ETag` usage where relevant).

7. [ ] Forms & validation
   - Use Reactive Forms with client-side validation that mirrors server DTO rules; surface validation errors and map `ValidationProblemDetails` to form field errors.

8. [ ] Error & loading UX
   - Global error handling UI (toasts/snackbars) and per-component loading states; consistent handling of `application/problem+json` responses.

9. [ ] Environment & dev proxy
   - Add environment configs for API base URL and a dev proxy to avoid CORS during local development.

10. [ ] Basic tests
   - Add a few focused unit tests for key services and one or two component tests (shallow) to verify core behaviors.

11. [ ] Developer ergonomics
   - Add README snippets for `npm install`, `ng serve`, and how to run frontend tests locally.

Acceptance criteria (MVP)
- `QueuesListComponent` with search + paging and a working `QueueEditComponent` form.
- `QueueService` and `AgentService` call the API and handle `ETag`/concurrency flows for updates/deletes.
- Basic unit tests and a simple dev workflow documented in the frontend README.

File: docs/part3-plan.md
