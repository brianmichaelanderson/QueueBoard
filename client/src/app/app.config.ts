import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withEnabledBlockingInitialNavigation } from '@angular/router';

import { routes } from './app.routes';
import { API_BASE_URL } from './app.tokens';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled'
      }),
      // Initial navigation: blocking mode ensures the router completes the initial
      // navigation before the app is considered stable. Preloading is intentionally
      // disabled (NoPreloading) for the MVP to keep initial bundle sizes small.
      withEnabledBlockingInitialNavigation(),
    )
  ]
};
