import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-queue-detail',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queue</h1>

        <section *if="!!item">
          <h2>{{ item.name }}</h2>
          <p class="muted">{{ item.description }}</p>
          <p><strong>ID:</strong> {{ id }}</p>
        </section>

        <section *if="!item">
          <p>Loading queue details...</p>
          <p *if="!id">No queue id provided.</p>
        </section>
      </main>
    </div>
  `,
  styles: [
    `:host { display:block }`,
    `.muted { color:#666 }`,
    `.page-title { margin-bottom: 0.5rem }`
  ]
})
export class QueueDetailComponent {
  private route = inject(ActivatedRoute);
  id: string | null = this.route.snapshot.paramMap.get('id');
  item: { id?: string; name?: string; description?: string } | null =
    (this.route.snapshot.data as any)?.initialData?.item ?? null;

  constructor() {
    console.log('QueueDetailComponent resolver initialData:', (this.route.snapshot.data as any)?.initialData);
  }
}
