import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueDetailComponent } from '../queues/queue-detail.component';

@Component({
  standalone: true,
  selector: 'app-admin-queue-detail',
  imports: [CommonModule, QueueDetailComponent],
  template: `
    <!-- Thin wrapper that reuses the queue detail component for admin flows -->
    <app-queue-detail></app-queue-detail>
  `
})
export class AdminQueueDetailComponent {}
