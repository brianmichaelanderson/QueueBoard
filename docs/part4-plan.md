## Part 4 — Angular routing + lazy loading (MVP plan)

Purpose: deliver the minimal routing + module structure to support the app's MVP. Keep implementation small, testable, and focused on making lazy modules, resolvers, and simple guards work end-to-end. Do not add any functionality beyond MVP (no new admin-only features, no UX experiments).

- [ ] 0. TDD workflow (project-wide)
   - [ ] 0.1 For each feature/task: write a single focused failing test (unit or integration) that asserts the intended behavior.
   - [ ] 0.2 Implement the minimal code required to make the test pass.
   - [ ] 0.3 Run focused tests until green; fix regressions introduced by the change.
   - [ ] 0.4 Refactor for clarity and run tests again.
   - [ ] 0.5 Commit with a descriptive message and ensure CI runs the focused test suite.

- [ ] 1. Routing & lazy modules
   - [ ] 1.1 Add app-level routes that lazy-load `AgentModule` at `/agents` and `AdminModule` at `/admin`.
      - [ ] 1.1.0 Write a failing router spec that asserts the lazy routes exist, then implement the routes to make the test pass.
        - [ ] 1.1.1 Scaffold `AgentModule` and `AdminModule` folders and route files (`agent.routes.ts`, `admin.routes.ts`).
        - [ ] 1.1.2 Add module barrels (`index.ts`) and export declarations in `client/src/app` routing.
        - [ ] 1.1.3 Update root routes (`app.routes.ts`) with lazy imports and a small comment documenting the intent.
   - [ ] 1.2 Verify `ng build` produces separate lazy chunks for the two modules.
      - [ ] 1.2.1 Add a quick `ng build --configuration=development` smoke step and check generated chunk names.
   - [ ] 1.3 Add an optional `PreloadingStrategy` entry (documented but disabled for MVP).

- [ ] 2. Route guards & auth shape (minimal)
   - [ ] 2.1 Implement a tiny `AuthService` stub and an `AuthGuard` that can be configured with role checks (`isAdmin` flag).
      - [ ] 2.1.0 Write a failing unit test for `AuthGuard` behavior (admin vs non-admin), then implement `AuthService`/`AuthGuard` to make it pass.
        - [ ] 2.1.1 Create `AuthService` stub with `isAdmin()` and `isAuthenticated()` helpers.
        - [ ] 2.1.2 Implement `AuthGuard` that accepts route-data (e.g., `roles: ['admin']`) and a small failure redirect `/login` or `/`.
   - [ ] 2.2 Protect `/admin/**` routes with the admin guard. Keep `AgentModule` routes readable without admin privileges.
      - [ ] 2.2.0 Write a failing integration test that asserts `/admin` routes are blocked for non-admin users, then implement guard wiring.
        - [ ] 2.2.1 Add route-data metadata to `/admin` routes and apply the guard.
        - [ ] 2.2.2 Hide admin nav links using `AuthService` (update header/nav component markup).

- [ ] 3. Resolvers / fetch-on-enter
   - [ ] 3.1 Add route resolvers for lists and details (re-use existing resolver pattern): list routes should resolve `{ items, total }`; detail routes should resolve `{ item }` when `:id` is present.
      - [ ] 3.1.0 Write a failing resolver unit test that asserts a list resolver returns `{ items, total }` and a detail resolver returns `{ item }`, then implement resolver scaffolding to make it pass.
        - [ ] 3.1.1 Create/standardize a small resolver base (list vs detail) and shared DTO mapping helpers.
        - [ ] 3.1.2 Ensure services set `rowVersion` from `ETag` consistently (verify `agent.service.ts` / `queue.service.ts`).
   - [ ] 3.2 Wire resolvers into lazy module routes so pages render with resolved `initialData` when navigated directly.
      - [ ] 3.2.0 Write a failing integration test that navigates directly to a `view/:id` route and expects `initialData.item` to be present, then wire resolvers into routes.
        - [ ] 3.2.1 Wire resolvers into the lazy route files and add a short comment about `initialData` shape.
        - [ ] 3.2.2 Add lightweight error handling in resolvers (navigate to list on 404).

- [ ] 4. Admin vs Agent routing policy (MVP decision)
   - [ ] 4.1 `AgentModule` provides read-only pages for both Agents and Queues (list + detail only). No edit/create actions exposed in this module.
      - [ ] 4.1.0 Write a failing test that asserts `AgentModule` routes do not expose edit/create routes, then implement route configs to satisfy it.
        - [ ] 4.1.1 If reusing shared components: update `agent.routes.ts`/`queues.routes.ts` comments to reflect read-only policy.
   - [ ] 4.2 `AdminModule` provides the edit/create flows. To avoid duplication, implement both of the following in the Admin routing layer:
      - [ ] 4.2.0 Write a failing test that admin routes render edit flows and are guarded; implement admin routing to make it pass.
        - [ ] 4.2.1 (A) Reuse existing `AgentList`/`AgentDetail`/`QueueList`/`QueueDetail` components by pointing `AdminModule` routes at those components and enabling edit controls via route-data or guard check.
          - [ ] 4.2.1.1 Update `admin.routes.ts` to point to shared components and include `route.data = { showEditButtons: true }` when appropriate.
          - [ ] 4.2.1.2 Ensure edit/create routes are defined under `AdminModule` only and guarded.
        - [ ] 4.2.2 (B) Provide thin `admin-*` wrapper components if you prefer explicit separation (these wrappers simply render the shared components and set route-data). Wrappers are optional; choose either A or B but keep both documented.
          - [ ] 4.2.2.1 Scaffold `admin-agent-list.component.ts` / `admin-agent-detail.component.ts` that render the shared components and set route-data.
          - [ ] 4.2.2.2 Add inline docs in route files pointing to the chosen approach.

- [ ] 5. Navigation and UX parity
   - [ ] 5.1 Ensure list items in both lists navigate to the `view/:id` detail routes.
      - [ ] 5.1.0 Write a failing component test that clicks a list item and expects navigation to `view/:id`, then implement routerLink/click behavior.
        - [ ] 5.1.1 Confirm `routerLink` points to `view/:id` and optionally make the whole row clickable if desired.
   - [ ] 5.2 Ensure edit/create routes are reachable only via admin routes (guarded) and that cancel/save behavior routes back to the correct view (detail or list) per MVP policy.
      - [ ] 5.2.0 Write a failing test that asserts save/cancel navigation semantics (create→list, edit→detail), then implement navigation logic.
        - [ ] 5.2.1 Verify save/cancel navigation semantics for both create and edit and document expected behavior in `docs/part4-plan.md`.

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

Next actions (after approval)
1. Break each checked high-level task into concrete steps and sub-steps.
2. Implement routing + guard + resolver wiring and run focused tests.
3. Iteratively address failures and document small deviations.
