import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueEditComponent } from '../queues/queue-edit.component';

@Component({
  standalone: true,
  selector: 'app-admin-queue-edit',
  imports: [CommonModule, QueueEditComponent],
  template: `
    <!-- Thin wrapper that reuses the queue edit component for admin flows -->
    <app-queue-edit></app-queue-edit>
  `
})
export class AdminQueueEditComponent {}
