import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AgentService } from '../services/agent.service';

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
export class AgentEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private agentService = inject(AgentService);

  form = this.fb.group({ name: ['', Validators.required] });
  id: string | null = null;
  isEdit = false;
  rowVersion?: string | undefined;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;

    if (this.isEdit) {
      // Load the agent from the service and patch the form
      this.agentService.getById(this.id!).subscribe(agent => {
        if (agent) {
          this.form.patchValue({ name: agent.name });
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
        (result as any).subscribe(() => this.router.navigate(['/agent']));
      } else {
        this.router.navigate(['/agent']);
      }
      return;
    }

    // For create flow, navigate back after saving — create not implemented yet
    console.log('Agent save', { id: this.id, ...payload });
    this.router.navigate(['/agent']);
  }

  cancel() {
    this.router.navigate(['/agent']);
  }
}
