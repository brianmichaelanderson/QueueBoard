import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-queue-edit',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queue Edit</h1>
        <p *ngIf="id; else create">Editing queue id: {{ id }}</p>
        <ng-template #create><p>Creating a new queue (placeholder).</p></ng-template>
      </main>
    </div>
  `
})
export class QueueEditComponent {
  id: string | null;
  constructor(private route: ActivatedRoute) {
    this.id = this.route.snapshot.paramMap.get('id');
  }
}
