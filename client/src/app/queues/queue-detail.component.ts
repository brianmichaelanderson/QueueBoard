import { Component, inject } from '@angular/core';
import { QueueDto } from '../shared/models/queue';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-queue-detail',
  imports: [CommonModule],
  template: `
    <!-- eslint-disable @angular-eslint/template/prefer-control-flow -->
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queue</h1>

        <section *ngIf="item; else loading">
          <div class="form-row">
            <label>Name</label>
            <div>{{ item.name }}</div>
          </div>

          <div class="form-row">
            <label>Description</label>
            <div class="muted">{{ item.description }}</div>
          </div>

          <div class="form-row">
            <label>ID</label>
            <div>{{ id }}</div>
          </div>

          <div class="actions">
            <button [disabled]="!showEditButtons" (click)="edit()">Edit</button>
            <button (click)="cancel()">Cancel</button>
          </div>
        </section>

        <ng-template #loading>
          <section>
            <p>Loading queue details...</p>
            <p *ngIf="!id">No queue id provided.</p>
          </section>
        </ng-template>
      </main>
    </div>
  `,
  styles: [
    `:host { display:block }`,
    `.muted { color:#666 }`,
    `.page-title { margin-bottom: 0.5rem }`
    ,`.form-row { margin-bottom: 0.75rem }`,
    `.actions { display:flex; gap:0.5rem; margin-top:1rem }`
  ]
})
export class QueueDetailComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  id: string | null = this.route.snapshot.paramMap.get('id');
  item: QueueDto | null = (this.route.snapshot.data as { initialData?: { item?: QueueDto } })?.initialData?.item ?? null;
  showEditButtons: boolean = !!((this.route.snapshot.data as any)?.showEditButtons);

  constructor() {
    console.log(
      'QueueDetailComponent resolver initialData:',
      (this.route.snapshot.data as { initialData?: { item?: QueueDto } })?.initialData
    );
  }

  edit() {
    if (this.id) {
      if (this.showEditButtons) {
        this.router.navigate(['/admin','queues','edit', this.id]);
      } else {
        this.router.navigate(['/queues', 'edit', this.id]);
      }
    }
  }

  cancel() {
    if (this.showEditButtons) {
      this.router.navigate(['/admin','queues']);
    } else {
      this.router.navigate(['/queues']);
    }
  }
}
