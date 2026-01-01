import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

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
export class AgentEditComponent {
  form: any;
  id: string | null = null;
  isEdit = false;

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private router: Router) {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;
    this.form = this.fb.group({ name: ['', Validators.required] });

    if (this.isEdit) {
      // placeholder: load existing values (replace with real service later)
      this.form.patchValue({ name: `Agent ${this.id}` });
    }
  }

  save() {
    console.log('Agent save', { id: this.id, ...this.form.value });
    this.router.navigate(['/agent']);
  }

  cancel() {
    this.router.navigate(['/agent']);
  }
}
