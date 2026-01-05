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
- After scaffolding, consider creating a minimal `src/app/app.routes.ts` and a lazy-loaded feature route for `queues` to match the project's standalone-component conventions.

Routing preloading strategy
- Routing preloading: `NoPreloading` (default) for the MVP to keep initial bundles small and only load feature code when the user navigates there. 

Zoneless recommendation
- **Recommendation:** Use a zoneless setup for the Angular 20 frontend and adopt Signals + `OnPush` change detection for components.
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

## Queues dev workflow (MVP)

A short checklist and commands to run the Queues feature locally with the backend.

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

- Run only the queues-related frontend tests (quick pattern):

```bash
# Run Karma for queues specs only
npm run test -- --include "src/app/queues/**" --watch=false
```

This snippet intentionally links to existing sections above and to `server/README.md` for backend-specific commands to avoid duplication.
```

Using the token makes it simple to swap API hosts for local dev, CI, or when running the backend in a container.

