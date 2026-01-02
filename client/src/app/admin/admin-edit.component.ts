import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-admin-edit',
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1>{{ isEdit ? 'Edit Admin' : 'Create Admin' }}</h1>
        <form [formGroup]="form" (ngSubmit)="save()">
          <label>
            Name
            <input formControlName="name" />
          </label>
          <div class="actions">
            <button type="submit" [disabled]="form.invalid">Save</button>
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
export class AdminEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  form = this.fb.group({ name: ['', Validators.required] });
  id: string | null = null;
  isEdit = false;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    if (this.isEdit) {
      this.form.patchValue({ name: `Admin ${this.id}` });
    }
  }

  save() {
    console.log('Admin save', { id: this.id, ...this.form.value });
    this.router.navigate(['/admin']);
  }

  cancel() {
    this.router.navigate(['/admin']);
  }
}
