import { FormBuilder } from '@angular/forms';
import { applyServerValidationErrors, ValidationProblemDetails } from './validation-mapper';

describe('applyServerValidationErrors', () => {
  let fb: FormBuilder;

  beforeEach(() => {
    fb = new FormBuilder();
  });

  it('returns false when there are no errors', () => {
    const form = fb.group({ name: [''] });
    const result = applyServerValidationErrors(form, {} as ValidationProblemDetails);
    expect(result).toBeFalsy();
    expect(form.errors).toBeNull();
    expect(form.get('name')?.errors).toBeNull();
  });

  it('applies field-level errors to controls and returns true', () => {
    const form = fb.group({ name: [''], description: [''] });
    const problem: ValidationProblemDetails = { errors: { name: ['Name is required'] } };

    const result = applyServerValidationErrors(form, problem);

    expect(result).toBeTrue();
    const nameErrors = form.get('name')?.errors as any;
    expect(nameErrors).toBeTruthy();
    expect(nameErrors.server).toContain('Name is required');
    expect(form.errors).toBeNull();
  });

  it('applies non-field errors to form-level errors and returns true', () => {
    const form = fb.group({ name: [''] });
    const problem: ValidationProblemDetails = { errors: { global: ['Something went wrong'] } };

    const result = applyServerValidationErrors(form, problem);

    expect(result).toBeTrue();
    expect(form.errors).toBeTruthy();
    expect((form.errors as any).server).toBeTruthy();
    expect((form.errors as any).server.global).toContain('Something went wrong');
  });
});
