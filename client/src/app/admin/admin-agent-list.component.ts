import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentListComponent } from '../agent/agent-list.component';

@Component({
  standalone: true,
  selector: 'app-admin-agent-list',
  imports: [CommonModule, AgentListComponent],
  template: `
    <!-- Thin wrapper that reuses the agent list component for admin flows -->
    <app-agent-list></app-agent-list>
  `
})
export class AdminAgentListComponent {}
