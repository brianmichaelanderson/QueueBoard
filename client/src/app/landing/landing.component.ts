import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-landing',
  imports: [CommonModule, RouterModule],
  template: `
    <main class="landing">
      <h1>QueueBoard</h1>
      <p>Welcome — pick a section to start.</p>

      <section>
        <h2>Agents</h2>
        <p><a routerLink="/agents">View Agents (read-only)</a></p>
        <p><a routerLink="/queues">View Queues (read-only)</a></p>
      </section>
    </main>
  `
})
export class LandingComponent {}
