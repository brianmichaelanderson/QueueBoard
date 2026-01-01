import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-agent-list',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Agents</h1>
        <p>Placeholder for 'AgentsListComponent'. Add list, search, and paging here.</p>
      </main>
    </div>
  `
})
export class AgentListComponent {}
