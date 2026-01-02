import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';

@Component({
  standalone: true,
  selector: 'app-queues-list',
  imports: [CommonModule, RouterModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queues</h1>

        <div class="controls">
          <input aria-label="Search queues" placeholder="Search queues" (input)="onSearch($event)" />
          <a class="create-link" [routerLink]="['/queues', 'create']">Create queue</a>
        </div>

        <ul class="queue-list">
          <li *for="let q of items" class="queue-item">
            <a [routerLink]="['/queues', 'edit', q.id]">{{ q.name }}</a>
            <p class="muted">{{ q.description }}</p>
          </li>
        </ul>

        <p *if="items.length === 0" class="empty">No queues found.</p>
      </main>
    </div>
  `,
  styles: [
    `:host { display: block; }`,
    `.page-title { margin-bottom: 0.5rem; }`,
    `.controls { display:flex; gap:1rem; align-items:center; margin-bottom:1rem }`,
    `.queue-list { list-style:none; padding:0; margin:0 }`,
    `.queue-item { padding:0.5rem 0; border-bottom:1px solid #eee }`,
    `.muted { color: #666; margin:0; font-size:0.9rem }`,
    `.empty { color:#666 }`
  ]
})
export class QueuesListComponent {
  private route = inject(ActivatedRoute);
  items: QueueDto[] = [];

  constructor() {
    const data = this.route.snapshot.data as { initialData?: { items?: QueueItem[] } };
    if (data?.initialData?.items) {
      this.items = data.initialData.items;
    } else {
      // Placeholder sample data for local dev
      this.items = [
        { id: '1', name: 'Support', description: 'Customer support queue' },
        { id: '2', name: 'Sales', description: 'Sales inquiries' }
      ];
    }
    console.log('QueuesListComponent resolver initialData:', data?.initialData);
  }

  onSearch(_event: Event) {
    // noop placeholder; debounce and server search will be implemented in 4.3
  }
}
