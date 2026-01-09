import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AgentService } from '../services/agent.service';
import { AuthService } from '../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { applyServerValidationErrors, ValidationProblemDetails } from '../shared/utils/validation-mapper';

@Component({
  standalone: true,
  selector: 'app-agent-edit',
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1>{{ isEdit ? 'Edit Agent' : 'Create Agent' }}</h1>
        <form [formGroup]="form" (ngSubmit)="save()">
          <label>
            First name
            <input formControlName="firstName" />
          </label>
          <label>
            Last name
            <input formControlName="lastName" />
          </label>
          <label>
            Email
            <input formControlName="email" />
          </label>
          <label>
            Active
            <input type="checkbox" formControlName="isActive" />
          </label>
          <div class="actions">
            <button type="submit" [disabled]="form.invalid || !isAdmin">Save</button>
            <button type="button" (click)="cancel()">Cancel</button>
          </div>
        </form>
      </main>
    </div>
  `,
  styles: [
    `:host { display:block }`,
    `.actions { margin-top: 1rem }`,
    `input { display:block; margin-top:0.5rem; padding:0.25rem }`
  ]
})
export class AgentEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private agentService = inject(AgentService);
  private auth = inject(AuthService);

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    isActive: [true]
  });
  id: string | null = null;
  isEdit = false;
  rowVersion?: string | undefined;
  isAdmin = true;

  ngOnInit(): void {
    // determine admin status synchronously where possible (AuthService.isAdmin may return Observable)
    try {
      const maybeIsAdmin = (this.auth as any).isAdmin;
      if (typeof maybeIsAdmin === 'function') {
        const result = maybeIsAdmin.call(this.auth);
        if (result && typeof result.subscribe === 'function') {
          (result as any).subscribe((v: boolean) => (this.isAdmin = v)).unsubscribe();
        } else {
          this.isAdmin = !!result;
        }
      }
    } catch {
      // fall back to true in case of unexpected errors
      this.isAdmin = true;
    }

    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    if (this.isEdit) {
      // Load the agent from the service and patch the form
      this.agentService.getById(this.id!).subscribe(agent => {
        if (agent) {
          this.form.patchValue({
            firstName: agent.firstName,
            lastName: agent.lastName,
            email: agent.email,
            isActive: agent.isActive ?? true
          });
          this.rowVersion = (agent as any).rowVersion ?? undefined;
        }
      });
    }
  }

  save() {
    const payload = { ...this.form.value };
    if (this.isEdit && this.id) {
      const result = this.agentService.update(this.id, payload, this.rowVersion as any);
      if (result && typeof (result as any).subscribe === 'function') {
        (result as any).subscribe({
          next: () => this.router.navigate(['/agents', 'view', this.id]),
          error: (err: unknown) => {
            if (err instanceof HttpErrorResponse && err.status === 400) {
              const body = (err as HttpErrorResponse).error as ValidationProblemDetails;
              applyServerValidationErrors(this.form, body);
            } else if (err instanceof HttpErrorResponse && err.status === 412) {
              alert('This agent has been modified by another user. Please reload and try again.');
            }
          }
        });
      } else {
          this.router.navigate(['/agents']);
      }
      return;
    }

    // Create flow: call service and navigate on success, handle validation errors
    const createResult = this.agentService.create(payload);
    if (createResult && typeof (createResult as any).subscribe === 'function') {
      (createResult as any).subscribe({
          next: (_created: any) => this.router.navigate(['/agents']),
        error: (err: unknown) => {
          if (err instanceof HttpErrorResponse && err.status === 400) {
            const body = (err as HttpErrorResponse).error as ValidationProblemDetails;
            applyServerValidationErrors(this.form, body);
          } else if (err instanceof HttpErrorResponse && err.status === 412) {
            alert('This agent has been modified by another user. Please reload and try again.');
          }
        }
      });
    } else {
        this.router.navigate(['/agents']);
    }
  }

  cancel() {
      if (this.isEdit && this.id) {
        this.router.navigate(['/agents', 'view', this.id]);
      } else {
        this.router.navigate(['/agents']);
      }
  }
}
