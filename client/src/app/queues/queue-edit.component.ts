import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-queue-edit',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">{{ isEdit ? 'Edit' : 'Create' }} Queue</h1>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" novalidate>
          <div class="form-row">
            <label for="name">Name</label>
            <input id="name" formControlName="name" />
            <div *if="form.controls.name.touched && form.controls.name.invalid" class="error">
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

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  isEdit = false;
  id: string | null = null;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    const data = this.route.snapshot.data as { initialData?: any };
    if (data?.initialData?.item) {
      this.form.patchValue(data.initialData.item);
    } else if (this.isEdit) {
      // placeholder: in a later step call QueueService.get(id)
      this.form.patchValue({ name: 'Sample queue', description: 'Loaded from placeholder' });
    }
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.value;
    if (this.isEdit) {
      console.log('Update queue', this.id, payload);
      // TODO: call QueueService.update(id, payload)
    } else {
      console.log('Create queue', payload);
      // TODO: call QueueService.create(payload)
    }

    this.router.navigate(['/queues']);
  }

  onCancel() {
    this.router.navigate(['/queues']);
  }
}

