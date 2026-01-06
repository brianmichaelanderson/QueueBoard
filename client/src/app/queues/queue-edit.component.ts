import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { QueueDto } from '../shared/models/queue';
import { QueueService } from '../services/queue.service';
import { HttpErrorResponse } from '@angular/common/http';
import { applyServerValidationErrors, ValidationProblemDetails } from '../shared/utils/validation-mapper';

@Component({
  standalone: true,
  selector: 'app-queue-edit',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <!-- eslint-disable @angular-eslint/template/prefer-control-flow -->
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">{{ isEdit ? 'Edit' : 'Create' }} Queue</h1>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" novalidate>
          <div class="form-row">
            <label for="name">Name</label>
            <input id="name" formControlName="name" />
            <div *ngIf="form.controls.name.touched && form.controls.name.invalid" class="error">
              Name is required
            </div>
          </div>

          <div class="form-row">
            <label for="description">Description</label>
            <textarea id="description" formControlName="description"></textarea>
          </div>

          <div class="actions">
            <button type="submit" [disabled]="form.invalid">Save</button>
            <button type="button" (click)="onCancel()">Cancel</button>
          </div>
        </form>
      </main>
    </div>
  `,
  styles: [
    `:host { display:block }`,
    `.form-row { margin-bottom: 0.75rem }`,
    `.error { color: #b91c1c; font-size: 0.9rem }`,
    `.actions { display:flex; gap:0.5rem; margin-top:1rem }`
  ]
})
export class QueueEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private queueService = inject(QueueService);

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  isEdit = false;
  id: string | null = null;
  private etag?: string;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    const data = this.route.snapshot.data as { initialData?: { item?: QueueDto } };
    if (data?.initialData?.item) {
      this.form.patchValue(data.initialData.item);
      this.etag = data.initialData.item.rowVersion;
    } else if (this.isEdit) {
      if (this.id) {
        this.queueService.get(this.id).subscribe({
          next: res => {
            if (res?.item) {
              this.form.patchValue(res.item);
              this.etag = res.etag;
            }
          }
        });
      }
    }
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.value as Partial<QueueDto>;
    if (this.isEdit && this.id) {
      this.queueService.update(this.id, payload, this.etag).subscribe({
        next: () => this.router.navigate(['/queues', 'view', this.id]),
        error: (err: unknown) => {
          if (err instanceof HttpErrorResponse && err.status === 400) {
            const body = err.error as ValidationProblemDetails;
            applyServerValidationErrors(this.form, body);
          } else if (err instanceof HttpErrorResponse && err.status === 412) {
            // ETag precondition failed — resource was modified by someone else
            alert('This queue has been modified by another user. Please reload and try again.');
          }
        }
      });
    } else {
      this.queueService.create(payload).subscribe({ next: (_created: QueueDto) => this.router.navigate(['/queues']) });
    }
  }

  onCancel() {
    if (this.isEdit && this.id) {
      this.router.navigate(['/queues', 'view', this.id]);
    } else {
      this.router.navigate(['/queues']);
    }
  }
}

