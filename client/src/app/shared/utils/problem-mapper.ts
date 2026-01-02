import { ValidationProblemDetails } from '../models/problem-details.model';

// Map server ValidationProblemDetails to a flat object suitable for Reactive Forms
export function mapValidationProblemToFormErrors(vpd: ValidationProblemDetails | null | undefined): Record<string, string> {
  const result: Record<string, string> = {};
  if (!vpd || !vpd.errors) return result;

  for (const key of Object.keys(vpd.errors)) {
    const arr = vpd.errors![key];
    if (Array.isArray(arr) && arr.length > 0) {
      result[key] = arr.join(' ');
    }
  }

  return result;
}

// Extract top-level non-field errors (e.g., 'Validation' or general detail)
export function extractNonFieldErrors(vpd: ValidationProblemDetails | null | undefined): string[] {
  if (!vpd) return [];
  const out: string[] = [];
  if (vpd.errors) {
    // common key for general validation messages
    if (vpd.errors['Validation']) out.push(...vpd.errors['Validation']);
  }
  if (vpd.detail) out.push(vpd.detail);
  return out;
}
