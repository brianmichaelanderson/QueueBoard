import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueuesListComponent } from '../queues/queues-list.component';

@Component({
  standalone: true,
  selector: 'app-admin-queue-list',
  imports: [CommonModule, QueuesListComponent],
  template: `
    <!-- Thin wrapper that reuses the queues list component for admin flows -->
    <app-queues-list></app-queues-list>
  `
})
export class AdminQueueListComponent {}
