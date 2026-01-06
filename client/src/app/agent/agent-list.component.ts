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
        <div class="agent-list-header">
          <input type="search" class="search-input" placeholder="Search agents" aria-label="Search agents" />
        </div>

        <ul class="agent-list" role="list">
          <li class="agent-item" role="listitem">
            <span class="agent-name">Agent A</span>
            <a class="agent-detail-link" href="/agents/view/1">Details</a>
            <a class="agent-edit-link" href="/agents/edit/1">Edit</a>
          </li>
          <li class="agent-item" role="listitem">
            <span class="agent-name">Agent B</span>
            <a class="agent-detail-link" href="/agents/view/2">Details</a>
            <a class="agent-edit-link" href="/agents/edit/2">Edit</a>
          </li>
        </ul>

        <nav class="pagination" aria-label="Pagination">
          <button class="page-prev" aria-label="Previous page">Prev</button>
          <span class="page-current" aria-live="polite">1</span>
          <button class="page-next" aria-label="Next page">Next</button>
        </nav>
      </main>
    </div>
  `
})
export class AgentListComponent {}
