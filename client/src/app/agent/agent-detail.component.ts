import { Component, inject } from '@angular/core';
import { AgentDto } from '../shared/models/agent.model';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-agent-detail',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Agent Detail</h1>
        <section *ngIf="item; else loading">
          <div>
            <label><strong>First name:</strong></label>
            <div>{{ item.firstName }}</div>
          </div>
          <div>
            <label><strong>Last name:</strong></label>
            <div>{{ item.lastName }}</div>
          </div>
          <div>
            <label><strong>Email:</strong></label>
            <div class="muted">{{ item.email }}</div>
          </div>
          <div>
            <label><strong>Active:</strong></label>
            <div>{{ item.isActive ? 'Yes' : 'No' }}</div>
          </div>
          <div style="margin-top:1rem">
            <div>
              <label><strong>ID:</strong></label>
              <div>{{ id }}</div>
            </div>
            <div style="margin-top:.5rem">
              <button (click)="edit()">Edit</button>
              <button (click)="cancel()">Cancel</button>
            </div>
          </div>
        </section>

        <ng-template #loading>
          <section>
            <p>Loading agent details...</p>
            <p *ngIf="!id">No agent id provided.</p>
          </section>
        </ng-template>
      </main>
    </div>
  `
})
export class AgentDetailComponent {
  private route = inject(ActivatedRoute);
  private router: Router = inject(Router);
  id: string | null = this.route.snapshot.paramMap.get('id');
  item: AgentDto | null = (this.route.snapshot.data as { initialData?: { item?: AgentDto } })?.initialData?.item ?? null;

  constructor() {
    // Helpful for debugging resolver-provided data
    // eslint-disable-next-line no-console
    console.log('AgentDetailComponent resolver initialData:', (this.route.snapshot.data as { initialData?: { item?: AgentDto } })?.initialData);
  }

  edit() {
    if (this.id) {
      this.router.navigate(['/agents', 'edit', this.id]);
    }
  }

  cancel() {
    this.router.navigate(['/agents']);
  }
}
