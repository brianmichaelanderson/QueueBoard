import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-agent-detail',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Agent Detail</h1>
        <p>Agent id: {{ id }}</p>
      </main>
    </div>
  `
})
export class AgentDetailComponent {
  private route = inject(ActivatedRoute);
  id: string | null = this.route.snapshot.paramMap.get('id');
}
