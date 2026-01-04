import { InjectionToken } from '@angular/core';

// Base API URL token. Defaults to '/api' when not provided by the app.
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  providedIn: 'root',
  factory: () => '/api'
});
