import { Component, inject } from '@angular/core';
import { AgentDto } from '../shared/models/agent.model';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-agent-detail',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Agent Detail</h1>

        <section *ngIf="item">
          <h2>{{ item.firstName }} {{ item.lastName }}</h2>
          <p class="muted">{{ item.email }}</p>
          <p><strong>ID:</strong> {{ id }}</p>
        </section>

        <section *ngIf="!item">
          <p>Loading agent details...</p>
          <p *ngIf="!id">No agent id provided.</p>
        </section>
      </main>
    </div>
  `
})
export class AgentDetailComponent {
  private route = inject(ActivatedRoute);
  id: string | null = this.route.snapshot.paramMap.get('id');
  item: AgentDto | null = (this.route.snapshot.data as { initialData?: { item?: AgentDto } })?.initialData?.item ?? null;

  constructor() {
    // Helpful for debugging resolver-provided data
    // eslint-disable-next-line no-console
    console.log('AgentDetailComponent resolver initialData:', (this.route.snapshot.data as { initialData?: { item?: AgentDto } })?.initialData);
  }
}
