import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgentDetailComponent } from '../agent/agent-detail.component';

@Component({
  standalone: true,
  selector: 'app-admin-agent-detail',
  imports: [CommonModule, AgentDetailComponent],
  template: `
    <!-- Thin wrapper that reuses the agent detail component for admin flows -->
    <app-agent-detail></app-agent-detail>
  `
})
export class AdminAgentDetailComponent {}
