## Part 4 — Angular routing + lazy loading (MVP plan)

Purpose: deliver the minimal routing + module structure to support the app's MVP. Keep implementation small, testable, and focused on making lazy modules, resolvers, and simple guards work end-to-end. Do not add any functionality beyond MVP (no new admin-only features, no UX experiments).

- [ ] 0. TDD workflow (project-wide)
   - [ ] 0.1 For each feature/task: write a single focused failing test (unit or integration) that asserts the intended behavior.
   - [ ] 0.2 Implement the minimal code required to make the test pass.
   - [ ] 0.3 Run focused tests until green; fix regressions introduced by the change.
   - [ ] 0.4 Refactor for clarity and run tests again.
   - [ ] 0.5 Commit with a descriptive message and ensure CI runs the focused test suite.

- [ ] 1. Routing & lazy modules
   - [x] 1.1 Add app-level routes that lazy-load `AgentModule` at `/agents` and `AdminModule` at `/admin`.
      - [x] 1.1.0 Write a failing router spec that asserts the lazy routes exist, then implement the routes to make the test pass.
        - [x] 1.1.1 Scaffold `AgentModule` and `AdminModule` folders and route files (`agent.routes.ts`, `admin.routes.ts`).
        - [x] 1.1.2 Add module barrels (`index.ts`) and export declarations in `client/src/app` routing.
        - [x] 1.1.3 Update root routes (`app.routes.ts`) with lazy imports and a small comment documenting the intent.
   - [x] 1.2 Verify `ng build` produces separate lazy chunks for the two modules.
      - [x] 1.2.1 Add a quick `ng build --configuration=development` smoke step and check generated chunk names.
   - [ ] 1.3 Add an optional `PreloadingStrategy` entry (documented but disabled for MVP).

- [ ] 2. Route guards & auth shape (minimal)
   - [x] 2.1 Implement a tiny `AuthService` stub and an `AuthGuard` that can be configured with role checks (`isAdmin` flag).
      - [x] 2.1.0 Write a failing unit test for `AuthGuard` behavior (admin vs non-admin), then implement `AuthService`/`AuthGuard` to make it pass.
        - [x] 2.1.1 Create `AuthService` stub with `isAdmin()` and `isAuthenticated()` helpers.
        - [x] 2.1.2 Implement `AuthGuard` that accepts route-data (e.g., `roles: ['admin']`) and a small failure redirect `/login` or `/`.
   - [x] 2.2 Protect `/admin/**` routes with the admin guard. Keep `AgentModule` routes readable without admin privileges.
         - [x] 2.2.0 Write a failing integration test that asserts `/admin` routes are blocked for non-admin users, then implement guard wiring.
            - [x] 2.2.1 Add route-data metadata to `/admin` routes and apply the guard.
   - [ ] 2.3 Hide admin nav links using `AuthService` (update header/nav component markup). 
      - [ ] 2.3.1 Write a failing component test that asserts admin links are hidden for non-admin users, then implement the conditional rendering.
      - [ ] 2.3.2 Update header/nav component template to use `AuthService.isAdmin()` to conditionally show/hide admin links.

 - [x] 3. Resolvers / fetch-on-enter (implemented in Part 3)
    - Note: the resolver functionality required for Part 4's MVP is already implemented in Part 3. See `docs/part3-plan.md` for the tasks that covered this behavior:
       - [x] `2.8.1` Add a resolver for pages needing initial data (the project chose the resolver pattern).
       - [x] `3.3.2` Add feature-level providers (services, resolvers) registered at the route level when appropriate.
       - [x] `4.2.1` / `5.2.1` Service implementations (`QueueService` and `AgentService`) that resolvers depend on (ETag handling is addressed in 4.3.3 and 5.2.2).
       - [x] `5.4.3` Add a resolver for initial list data (agents feature mirrors queues resolver).
    - Rationale: feature-level `ResolveFn`s exist (e.g. `agents.resolver.ts`, `queues.resolver.ts`) and are wired into the lazy routes with `resolve: { initialData: ... }`. They return `{ items, total }` for list routes and `{ item }` for detail routes and are exercised by existing integration/component tests. The planned shared resolver base is a refactor and not required for MVP.

 - [x] 4. Admin vs Agent routing policy (MVP decision)
    - [x] 4.1 `AgentModule` provides read-only pages for both Agents and Queues (list + detail only). No edit/create actions exposed in this module.
       - [x] 4.1.0 Write a failing test that asserts `AgentModule` routes do not expose edit/create routes, then implement route configs to satisfy it.
      - [x] 4.1.1 If reusing shared components: update `agent.routes.ts`/`queues.routes.ts` comments to reflect read-only policy.
   - [x] 4.2 `AdminModule` provides the edit/create flows using thin `admin-*` wrapper components that render the existing admin components in `src/app/admin` and set `route.data = { showEditButtons: true }`.
    - [x] 4.2.0 Write a failing test that admin routes render edit flows and are guarded; implement admin routing to make it pass.
      - [x] 4.2.1 Use existing admin components in `src/app/admin` (e.g., `admin-agent-list`, `admin-agent-detail`, `admin-agent-edit`) and create thin wrapper components that render these shared components and set route-data.
      - [x] 4.2.2 Ensure edit/create routes are defined under `AdminModule` only and protected with `AuthGuard`.
      - [x] 4.2.3 Add inline docs in `admin.routes.ts` showing the route-data example and the wrapper pattern.

- [ ] 5. Navigation and UX parity
   - [ ] 5.1 Ensure list items in both lists navigate to the `view/:id` detail routes.
      - [ ] 5.1.0 Write a failing component test that clicks a list item and expects navigation to `view/:id`, then implement routerLink/click behavior.
        - [ ] 5.1.1 Confirm `routerLink` points to `view/:id` and optionally make the whole row clickable if desired.
   - [ ] 5.2 Ensure edit/create routes are reachable only via admin routes (guarded) and that cancel/save behavior routes back to the correct view (detail or list) per MVP policy.
      - [ ] 5.2.0 Write a failing test that asserts save/cancel navigation semantics (create→list, edit→detail), then implement navigation logic.
      - [ ] 5.2.1 Verify save/cancel navigation semantics for both create and edit and document expected behavior in `docs/part4-plan.md`.
      - [ ] 5.2.2 Write a failing component/unit test that asserts Edit/Create controls are disabled for non-admin users (stub `AuthService.isAdmin()` → false).
        - [ ] 5.2.3 Disable Edit/Create UI controls for non-admins (show the controls but keep them disabled when `AuthService.isAdmin()` is false). Implement the UI change to make the test pass.

- [ ] 6. Tests & verification (MVP smoke tests)
   - [ ] 6.1 Add route-level unit/integration tests that assert:
      - [ ] 6.1.1 lazy modules load (one test that imports router and asserts lazy config),
      - [ ] 6.1.2 `/admin` routes are protected by the guard,
      - [ ] 6.1.3 detail resolvers provide `initialData.item` for `view/:id` routes,
      - [ ] 6.1.4 list items route to `view/:id`.
      - [ ] 6.1.5 Add test harness utilities (test router config helper, `AuthService` mock, resolver mock) to `client/src/test-helpers/`.
   - [ ] 6.2 Run the focused component/spec test suites (queues + agents) and fix any failing expectations caused by routing/data changes.
      - [ ] 6.2.1 Add one smoke integration test that loads the router and asserts lazy route config exists (run in CI as a smoke check).

- [ ] 7. Documentation
   - [ ] 7.1 Update `QueueBoard-outline.md` to record the `AgentModule` read-only vs `AdminModule` edit policy and reference this plan file.
      - [ ] 7.1.1 Add a one-line reference in `QueueBoard-outline.md` pointing to `docs/part4-plan.md` and a short sentence describing the Agent vs Admin policy.
   - [ ] 7.2 Add short notes in module route files (comments) describing whether they reuse shared components or use wrappers.
      - [ ] 7.2.1 Update `client/README.md` with a quick dev note about lazy routes and how to run focused tests.

Notes and constraints
- Keep everything minimal: this phase is about routing/guard/resolver wiring, not UX or new features.
- Prefer reusing components to reduce duplication. If you foresee immediate admin-only differences (bulk actions, extra columns), add thin wrappers instead of duplicating templates.
- Defer full auth/token flows to Part 5 — use a stubbed `AuthService` and `AuthGuard` for routing tests in Part 4.

Acceptance criteria (MVP)
- Navigating to `/agents` and `/agents/view/:id` loads the agent list and detail pages from the lazy `AgentModule` bundle.
- Navigating to `/admin` loads the admin bundle; admin edit routes are guarded and use the same detail/list components (or thin wrappers) to perform edit flows.
- Tests assert lazy loading, resolver behavior, guard enforcement, and list→detail navigation.

