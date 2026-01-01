import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-queues',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queues</h1>
        <p>This is a placeholder for 'QueuesListComponent'. Implement list, search and paging here.</p>
      </main>
    </div>
  `,
  styles: [
    `:host { display: block; }`,
    `.page-title { margin-bottom: 0.5rem; }`
  ]
})
export class QueuesComponent {
  constructor(private route: ActivatedRoute) {
    // Log resolver-provided initial data for verification during development
    const data = this.route.snapshot.data as { initialData?: any };
    console.log('QueuesComponent resolver initialData:', data?.initialData);
  }
}
