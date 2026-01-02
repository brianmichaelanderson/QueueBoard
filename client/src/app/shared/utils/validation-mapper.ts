import { FormGroup } from '@angular/forms';

export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  traceId?: string;
  errors?: Record<string, string[]>;
}

// Apply server-side validation errors (RFC 7807 ValidationProblemDetails) to an Angular form.
export function applyServerValidationErrors(form: FormGroup, problem: ValidationProblemDetails) {
  const errors = problem?.errors;
  if (!errors) return;

  // Clear previous server errors
  const currentFormErrors = form.errors ?? null;
  if (currentFormErrors && typeof currentFormErrors === 'object' && currentFormErrors['server']) {
    form.setErrors(null);
  }

  let anyApplied = false;
  for (const key of Object.keys(errors)) {
    const messages = errors[key];
    const control = form.get(key);
    if (control) {
      control.setErrors({ server: messages.join(' ') });
      anyApplied = true;
    } else {
      // non-field error: attach to form-level under `server` key
      const prev = form.errors ?? {};
      prev['server'] = { ...(prev['server'] ?? {}), [key]: messages };
      form.setErrors(prev);
      anyApplied = true;
    }
  }

  return anyApplied;
}
