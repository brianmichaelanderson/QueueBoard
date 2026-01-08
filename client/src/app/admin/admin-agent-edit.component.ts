import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentEditComponent } from '../agent/agent-edit.component';

@Component({
  standalone: true,
  selector: 'app-admin-agent-edit',
  imports: [CommonModule, AgentEditComponent],
  template: `
    <!-- Thin wrapper that reuses the agent edit component for admin flows -->
    <app-agent-edit></app-agent-edit>
  `
})
export class AdminAgentEditComponent {}
