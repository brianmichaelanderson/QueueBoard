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
      - 2.4.2 [x] Add `agent` lazy route
      - 2.4.3 [x] Add `admin` lazy route
      - 2.4.4 [x] Create feature-level `*.routes.ts` files for each feature (queues + agent + admin implemented)
    - 2.5 [x] NotFound & fallback route
       - 2.5.1 [x] Implement a `NotFoundComponent` or simple page
       - 2.5.2 [x] Add fallback route to redirect unknown paths to NotFound or root
    - 2.6 [x] Add a simple route guard stub
       - 2.6.1 [x] Implement a lightweight `AuthGuard` stub that returns `true` in dev
       - 2.6.2 [x] Attach guard to protected admin routes
    - 2.7 [x] Configure router options
       - 2.7.1 [x] Enable `scrollPositionRestoration` and `anchorScrolling`
       - 2.7.2 [x] Set `initialNavigation` and document preloading strategy choice in client/README
    - 2.8 [x] Route-level data fetching & resolvers (optional)
       - 2.8.1 [x] Add a resolver for pages needing initial data (chose the resolver pattern — the alternative "fetch-on-enter" was not used)
       - 2.8.2 [ ] Ensure resolvers return minimal payloads and handle errors gracefully (deferred — requires backend `QueueService`)
    - 2.9 [x] Verify lazy-loading and routing behavior
       - 2.9.1 [x] Run the dev server and navigate through lazy routes
       - 2.9.2 [x] Verify lazy bundles are created on demand (network/devtools)
       - 2.9.3 [x] Confirm default route and NotFound behavior

3. [ ] Feature scaffolding: Provide full feature scaffolding for `admin` and `agent` following the project's standalone-component conventions. Each feature should be self-contained and lazy-loadable.
   - 3.1 [x] Create feature folders and top-level route files
      - 3.1.1 [x] Add `admin/*.routes.ts` and `agent/*.routes.ts` (feature-level route arrays)
      - 3.1.2 [x] Add placeholder `List` and `Detail` standalone components for each feature (`*-list.component.ts`, `*-detail.component.ts`)
      - 3.1.3 [x] Add `Edit` component(s) where the feature supports create/edit flows (`*-edit.component.ts`)
   - 3.2 [x] Feature services, models and shared types
      - 3.2.1 [x] Add `AgentService` and `AdminService` skeletons that will wrap `HttpClient` calls (implemented as placeholders at `client/src/app/services`)
      - 3.2.2 [x] Add typed models/interfaces for `Agent/Queue` and feature-specific DTOs in a shared models area (`client/src/app/shared/models`)
      - 3.2.3 [x] Add basic error/response mapping utilities (to map `ValidationProblemDetails` later)
   - 3.3 [x] Lazy-loading, route providers and guards
      - 3.3.1 [x] Ensure `app.routes.ts` lazy-loads feature routes via `loadChildren`/`loadComponent`
      - 3.3.2 [x] Add feature-level providers (services, resolvers) registered at the route level when appropriate
      - 3.3.3 [x] Wire route guards for protected feature areas (admin guard already stubbed; added `authGuard` and applied it to admin routes)
   - 3.4 [ ] Styling, assets and accessibility
      - 3.4.1 [x] Add feature-scoped SCSS files and import shared CSS variables (`styles.scss`) where needed
      - 3.4.2 [x] Verify components meet basic accessibility requirements (semantic HTML, ARIA where needed)
         - 3.4.2.1 [x] Automated audits: `axe-core` scan returned no violations; report saved at `client/axe-home.json`
         - 3.4.2.2 [x] Lighthouse accessibility checks: no blocking accessibility issues; report saved at `lighthouse-home.json`
         - 3.4.2.3 [ ] Manual accessibility testing (keyboard navigation, screen reader smoke test) — pending
   - 3.5 [ ] Tests and verification
      - 3.5.1 [ ] Add unit tests for feature components (shallow) and services (skeleton)
      - 3.5.2 [ ] Add a simple integration/interaction test that navigates to each feature route (optional)
      - 3.5.3 [x] Acceptance: automated checks — ran `axe` and Lighthouse against `/`, `/admin`, and `/agent`; verified lazy chunk network requests, resolver console logs, and route guards are wired; manual accessibility smoke tests deferred


4. [ ] Queues UI
    - 4.1 [ ] Components & routing
       - 4.1.1 [x] Add `QueuesListComponent` (standalone) — list view with item links
       - 4.1.2 [ ] Add `QueueEditComponent` (standalone) — create / edit reactive form
       - 4.1.3 [ ] Add `QueueDetailComponent` (optional) — view-only detail page
       - 4.1.4 [x] Create `queues.routes.ts` and wire lazy route in `app.routes.ts`

    - 4.2 [ ] Forms, validation & concurrency
       - 4.2.1 [ ] Use Reactive Forms in `QueueEditComponent` with proper initial state
       - 4.2.2 [ ] Map `ValidationProblemDetails` responses to field-level errors
       - 4.2.3 [ ] Implement `ETag` handling: read ETag on GET, send `If-Match` on update

    - 4.3 [ ] Search, paging & UX
       - 4.3.1 [ ] Add search input with debounce and query binding
       - 4.3.2 [ ] Add pagination controls and pageSize support (server-side page params)
       - 4.3.3 [ ] Implement loading indicators, skeletons, and empty states

    - 4.4 [ ] Services & API integration
       - 4.4.1 [ ] Implement `QueueService` methods: `list`, `get`, `create`, `update`, `delete`
       - 4.4.2 [ ] Use typed DTOs and centralize API base URL in environment config
       - 4.4.3 [ ] Register `QueueService` as a route-level provider when appropriate

    - 4.5 [ ] Accessibility, styling & assets
       - 4.5.1 [ ] Add feature-scoped SCSS and import shared variables from `styles.scss`
       - 4.5.2 [ ] Ensure semantic markup, keyboard focus order, and accessible names
       - 4.5.3 [ ] Run automated `axe` and Lighthouse checks for the queues views

    - 4.6 [ ] Tests & verification
       - 4.6.1 [ ] Unit tests for `QueueService` and components (shallow)
       - 4.6.2 [ ] Integration test: navigate to `/queues`, exercise search + pagination
       - 4.6.3 [ ] Acceptance: confirm lazy chunk request when navigating to `/queues` and resolver/guard wiring

    - 4.7 [ ] Docs & dev notes
       - 4.7.1 [ ] Add README snippet describing queues dev workflow and test commands

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
