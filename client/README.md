# QueueBoard — Frontend (client)

This README documents local setup steps to create and run the Angular frontend for QueueBoard.

> These commands assume macOS / Zsh and that you will create the app in `client/` under the repo root.

1) Verify Node and npm are available
```bash
node -v && npm -v
```

2) Create the Angular application in `client/` (Angular v20)
```bash
# Run from the repo root
npx -p @angular/cli@20 ng new client --routing --style=scss --skip-git
cd client
```
This scaffolds a new Angular 20 application in the `client/` folder with routing and SCSS.

3) Add Angular ESLint integration (Angular 20 compatible)
```bash
# from inside client/
npx -p @angular/cli@20 ng add @angular-eslint/schematics
```
Installs and configures ESLint for Angular projects and converts default TSLint or legacy configs where applicable.

4) Install Prettier for code formatting
```bash
# install formatting helper
npm install --save-dev prettier
```
Installs formatting (`prettier`) to keep source formatted consistently.

5) Add a proxy for local API calls (create `proxy.conf.json` in `client/`)
```json
{
  "/api": {
    "target": "http://localhost:8080",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  }
}
```
This tells the dev server to proxy `/api` requests to the backend to avoid CORS during local development.

-6) Install dependencies and start the dev server
```bash
npm install
npm run start
```
Installs node modules and runs the dev server at `http://localhost:4200` (proxied `/api` -> backend).

Development server
```bash
# recommended (uses proxy)
npm run start

# fallback (direct Angular CLI)
ng serve
```

Code scaffolding
Use the Angular CLI to generate components, directives, pipes, etc.:
```bash
ng generate component component-name
```

Building
```bash
ng build
```

Running unit tests
```bash
ng test
```

End-to-end tests (optional)
```bash
ng e2e
```

Notes
- If you prefer `pnpm` or `yarn`, substitute the package manager commands.

Routing preloading strategy
- Routing preloading: `NoPreloading` (default) for the MVP to keep initial bundles small and only load feature code when the user navigates there. 

Admin wrapper pattern
- Admin edit/create flows are implemented via thin wrapper components under `src/app/admin` that reuse shared feature components (agents/queues).
  - Rationale: wrappers avoid duplicating templates while allowing admin-only route metadata and providers to be applied at the route level. This keeps
    `AgentModule` read-only while `AdminModule` exposes guarded edit/create flows.
  - Files (examples): `src/app/admin/admin.routes.ts`, `src/app/admin/admin-agent-list.component.ts`, `src/app/admin/admin-agent-detail.component.ts`, `src/app/admin/admin-agent-edit.component.ts`.
  - Route-data conventions used:
    - `roles: ['admin']` — indicates the route requires the admin role (checked by guards).
    - `showEditButtons: true` — hint for shared components to reveal admin-only edit/create UI when rendered under admin routes.
  - Tests: see `src/app/admin/admin.routes.blocking.integration.spec.ts` and `src/app/admin/admin.routes.editing.integration.spec.ts` for guard and routing coverage. Add wrapper rendering tests when you introduce admin-specific UI.

Zoneless implementation
- Use a zoneless setup for the Angular 20 frontend and adopt Signals + `OnPush` change detection for components.
- **Why:** Zoneless apps avoid Zone.js implicit change-detection side-effects, are easier to reason about, and pair naturally with Signals (Angular's forward direction) for predictable, high-performance UI updates.
- **Caveats:** Some third-party libraries expect Zone.js; test any library that patches async APIs. Zoneless requires discipline (use `ChangeDetectionStrategy.OnPush`, Signals or manual `markForCheck()`), but is a small upfront cost for better runtime behavior.
- **How to enable (high level):** remove or avoid importing `zone.js` in `polyfills.ts`, bootstrap with `bootstrapApplication()` and ensure components use `OnPush` or Signals-based reactivity.

API base URL token (frontend)

The frontend exposes an injectable `API_BASE_URL` token that centralizes the HTTP API base path used by services (for example, the `QueueService`). The application reads the value from the environment (`client/src/environments/environment.ts`) and provides it at bootstrap in `client/src/app/app.config.ts`.

Test override example (unit tests / TestBed):

```ts
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { API_BASE_URL } from 'src/app/app.tokens';

TestBed.configureTestingModule({
  imports: [HttpClientTestingModule],
  providers: [
    { provide: API_BASE_URL, useValue: 'http://localhost:8080' }
  ]
});
```

Bootstrap/provider example (app startup):

```ts
import { API_BASE_URL } from './app/app.tokens';
import { environment } from '../environments/environment';

bootstrapApplication(AppComponent, {
  providers: [
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl }
  ]
});

```
## Queues dev workflow (MVP)

A short checklist and commands to run the Queues feature locally with the backend.

- Start the backend (Docker recommended):

```
bash
# from repo root
docker compose up -d --build api
```

- Reset or seed the dev database (server):

```bash
./scripts/reset-db.sh
# or see server/README.md for EF / Docker-based reset instructions
```

- Start the frontend dev server (with proxy):

```bash
cd client
npm install
npm run start
```

- If your backend is available on a non-default host/port (e.g. Docker binds the API to `http://localhost:8080`), override the `API_BASE_URL` in tests or the environment. Example TestBed override:

```ts
TestBed.configureTestingModule({
  providers: [ { provide: API_BASE_URL, useValue: 'http://localhost:8080' } ]
});
```

- Run only the queues-related frontend tests (quick pattern):

```bash
# Run Karma for queues specs only
npm run test -- --include "src/app/queues/**" --watch=false
```

This snippet intentionally links to existing sections above and to `server/README.md` for backend-specific commands to avoid duplication.

## Agents dev workflow (MVP)

A short checklist and commands to run the Agents feature locally with the backend (mirrors the Queues section).

- Start the backend (Docker recommended):

```bash
# from repo root
docker compose up -d --build api
```

- Reset or seed the dev database (server):

```bash
./scripts/reset-db.sh
# or see server/README.md for EF / Docker-based reset instructions
```

- Start the frontend dev server (with proxy):

```bash
cd client
npm install
npm run start
```

- If your backend is available on a non-default host/port (e.g. Docker binds the API to `http://localhost:8080`), override the `API_BASE_URL` in tests or the environment. Example TestBed override:

```ts
TestBed.configureTestingModule({
  providers: [ { provide: API_BASE_URL, useValue: 'http://localhost:8080' } ]
});
```

- Run only the agents-related frontend tests (quick pattern):

```bash
# Run Karma for agents specs only
npm run test -- --include "src/app/agent/**" --watch=false
```

Using the token makes it simple to swap API hosts for local dev, CI, or when running the backend in a container.

## Landing & Admin demo

- The app includes a simple Landing page that exposes both read-only and admin demo links. Admin links call a small dev helper (`AuthService.becomeAdmin()`) to mark the session as an admin and then navigate to the guarded admin routes (for example `/admin` or `/admin/queues`). This is a convenience for local development only — it is not a production auth flow.

## Agent vs Admin routing policy

- `AgentModule` (`/agents`) is intended to be read-only: list and detail views only.
- `AdminModule` (`/admin`) exposes edit/create flows. Edit/create are implemented using thin `admin-*` wrapper components that reuse the shared feature components and set `route.data.showEditButtons = true` so the same components render admin UI when used under admin routes.
- Routing ordering note: specific admin child paths (for example `queues`, `queues/:id`) must appear before generic parameter routes such as `:id` to avoid accidental route capture (see `src/app/admin/admin.routes.ts`).

## Dev auth helpers

- `AuthService` exposes development helpers useful for local testing:
  - `becomeAdmin()` — mark the session as admin (used by Landing admin links).
  - `becomeUser()` — clear admin state.
  - `isAdmin()` — observable checking admin state.

These are intended for local development and tests; they simplify exploring guarded flows without a full auth implementation.

## Testing notes (client)

- The frontend test setup prefers a zoneless approach. Tests avoid relying on Zone.js by using `provideZonelessChangeDetection()` and small test helpers in `src/app/test-helpers`.
- Prefer asserting `routerLink` values or spying on `Router.navigate` in unit tests rather than triggering full Router navigation; this keeps specs fast and avoids NgZone fragility.
- When adding admin-related tests, mock or provide `ActivatedRoute.snapshot.data.showEditButtons = true` so shared components render admin UI in unit tests.

## Troubleshooting

- If navigating to `/admin/queues` renders an unexpected agent detail page, check `src/app/admin/admin.routes.ts` route ordering — ensure `queues` routes are declared before the generic `:id` admin route.
- If unit tests fail due to Router/NgZone issues, prefer shallow assertions (routerLink) or spy on `Router.navigate` and use `provideZonelessChangeDetection()` in TestBed providers.

