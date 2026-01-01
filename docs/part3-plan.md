# Part 3 — Angular SPA Plan (High-level)

Purpose
- High-level tasks to implement the Angular SPA core described in `QueueBoard-outline.md` (Part 3). This is an MVP-focused checklist — no deep substeps or implementation details here.

Goals
- Provide a maintainable standalone-component-based Angular app with list/detail pages, create/edit forms, and clear API integration for queues and agents.

Top-level tasks

1. [x] Project setup
    - 1.1 [x] Verify workspace presence
       - 1.1.1 [x] Check for `client/` folder
       - 1.1.2 [x] Verify `angular.json` or `package.json` looks like an Angular workspace
    - 1.2 [x] Initialize the Angular workspace
       - 1.2.1 [x] Run `ng new client --routing --style=scss --skip-git` (or equivalent)
       - 1.2.2 [x] Choose zoneless setup where appropriate (Signals + `bootstrapApplication()`)
       - 1.2.3 [x] Confirm initial scaffold files present (`src/`, `e2e/`, `tsconfig.json`)
    - 1.3 [x] Install runtime and dev dependencies
       - 1.3.1 [x] Add Angular runtime dependencies (`@angular/core`, `@angular/common`, `@angular/router`, `rxjs`, `tslib`)
       - 1.3.2 [x] Add Angular CLI and compiler dev deps (`@angular/cli`, `@angular/compiler-cli`, `typescript`)
       - 1.3.3 [x] Install formatting/linting deps (`eslint`, `@angular-eslint/*`, `prettier`) and run `npm install`
    - 1.4 [x] Add linting & formatting
       - 1.4.1 [x] Add `@angular-eslint` via schematic or config files
       - 1.4.2 [x] Add `.eslintrc.cjs` and `prettier` config files (`.prettierrc`)
       - 1.4.3 [x] Add `lint` and `format` npm scripts (e.g., `ng lint`, `prettier --write`) in `package.json`
    - 1.5 [ ] Developer hooks and quality helpers (optional)
       - 1.5.1 [ ] Add `husky` and install hooks (optional)
       - 1.5.2 [ ] Add `lint-staged` configuration to run format/lint on staged files (optional)
       - 1.5.3 [ ] Add `prepare` script to `package.json` if using Husky (optional)
    - 1.6 [x] Add npm script set for local dev
       - 1.6.1 [x] `start` — dev server with proxy (`ng serve --proxy-config proxy.conf.json`)
       - 1.6.2 [x] `build` — `ng build`
       - 1.6.3 [x] `test` — `ng test`
       - 1.6.4 [x] `lint` / `lint:fix` and `format` scripts
    - 1.7 [x] Local dev convenience and CI hints
       - 1.7.1 [x] Add `proxy.conf.json` for local API proxying
       - 1.7.2 [x] Add README snippets for `npm install` and `npm run start`
       - 1.7.3 [x] Document CI commands (lint/test/build) in README

2. [ ] App shell & routing: Implement the root app shell and global routing. Configure lazy-loaded feature routes for `admin` and `agent` feature areas.
   - 2.1 [x] Create the root app shell — Add a minimal `AppShell`/`AppComponent` that provides header, footer and a `router-outlet` (or `Outlet` for standalone bootstrap). Keep layout responsive and accessible.
    - 2.2 [x] Global styles & layout container
       - 2.2.1 [x] Create `styles.scss` with base layout variables and container rules
      - 2.2.2 [x] Add shared CSS utilities and a container class used by the app shell
    - 2.3 [x] Create `app.routes.ts` (top-level router file)
       - 2.3.1 [x] Define top-level routes and lazy-route placeholders
       - 2.3.2 [x] Add default redirect (e.g., `/` → `/queues`)
    - 2.4 [ ] Add lazy-loaded feature routes
       - 2.4.1 [x] Add `queues` lazy route using standalone `loadChildren`/`loadComponent`
       - 2.4.2 [ ] Add `agent` lazy route
       - 2.4.3 [ ] Add `admin` lazy route
       - 2.4.4 [x] Create feature-level `*.routes.ts` files for each feature
    - 2.5 [x] NotFound & fallback route
       - 2.5.1 [x] Implement a `NotFoundComponent` or simple page
       - 2.5.2 [x] Add fallback route to redirect unknown paths to NotFound or root
    - 2.6 [ ] Add a simple route guard stub
       - 2.6.1 [ ] Implement a lightweight `AuthGuard` stub that returns `true` in dev
       - 2.6.2 [ ] Attach guard to protected admin routes
    - 2.7 [ ] Configure router options
       - 2.7.1 [ ] Enable `scrollPositionRestoration` and `anchorScrolling`
       - 2.7.2 [ ] Set `initialNavigation` and document preloading strategy choice
    - 2.8 [ ] Route-level data fetching & resolvers (optional)
       - 2.8.1 [ ] Add a resolver or fetch-on-enter pattern for pages needing initial data
       - 2.8.2 [ ] Ensure resolvers return minimal payloads and handle errors gracefully
    - 2.9 [ ] Verify lazy-loading and routing behavior
       - 2.9.1 [ ] Run the dev server and navigate through lazy routes
       - 2.9.2 [ ] Verify lazy bundles are created on demand (network/devtools)
       - 2.9.3 [ ] Confirm default route and NotFound behavior

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
