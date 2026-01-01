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

4) Install Prettier, Husky and lint-staged and configure a pre-commit hook
```bash
npm install --save-dev prettier husky lint-staged
npm pkg set scripts.prepare="husky install"
npx husky install
npx husky add .husky/pre-commit "npx lint-staged"
```
Adds formatting (`prettier`) and a Git pre-commit hook (`husky` + `lint-staged`) to run linters/formatters on staged files.

5) Example `lint-staged` configuration (create `.lintstagedrc.cjs`)
```js
module.exports = {
  'src/**/*.{ts,html,scss}': ['npm run format', 'npm run lint:fix']
};
```
Runs formatting and lint fixes only on staged source files to keep commits clean and fast.

6) Add a proxy for local API calls (create `proxy.conf.json` in `client/`)
```json
{
  "/api": {
    "target": "http://localhost:5000",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  }
}
```
This tells the dev server to proxy `/api` requests to the backend to avoid CORS during local development.

7) Useful `package.json` scripts (run these from `client/` or edit `package.json`)
```bash
npm pkg set scripts.start="ng serve --proxy-config proxy.conf.json"
npm pkg set scripts.build="ng build"
npm pkg set scripts.test="ng test"
npm pkg set scripts.lint="ng lint"
npm pkg set scripts.format="prettier --write \"src/**/*.{ts,html,scss}\""
# Add "lint:fix": "ng lint --fix" manually to package.json
```
Adds convenient scripts for starting the dev server (with proxy), building, testing, linting and formatting.

8) Install dependencies and start the dev server
```bash
npm install
npm run start
```
Installs node modules and runs the dev server at `http://localhost:4200` (proxied `/api` -> backend).

Notes
- If you prefer `pnpm` or `yarn`, substitute the package manager commands.
- After scaffolding, consider creating a minimal `src/app/app.routes.ts` and a lazy-loaded feature route for `queues` to match the project's standalone-component conventions.
- If you'd like, I can scaffold `client/` here in the repo instead of you running these commands locally.

Zoneless recommendation
- **Recommendation:** Use a zoneless setup for the Angular 20 frontend and adopt Signals + `OnPush` change detection for components.
- **Why:** Zoneless apps avoid Zone.js implicit change-detection side-effects, are easier to reason about, and pair naturally with Signals (Angular's forward direction) for predictable, high-performance UI updates.
- **Caveats:** Some third-party libraries expect Zone.js; test any library that patches async APIs. Zoneless requires discipline (use `ChangeDetectionStrategy.OnPush`, Signals or manual `markForCheck()`), but is a small upfront cost for better runtime behavior.
- **How to enable (high level):** remove or avoid importing `zone.js` in `polyfills.ts`, bootstrap with `bootstrapApplication()` and ensure components use `OnPush` or Signals-based reactivity.

