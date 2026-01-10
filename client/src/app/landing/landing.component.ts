import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';

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
      
      <section>
        <h2>Admins</h2>
        <p><a href="/admin" (click)="enterAdmin($event, '/admin')">View Agents (admin — edit enabled)</a></p>
        <p><a href="/admin/queues" (click)="enterAdmin($event, '/admin/queues')">View Queues (admin — edit enabled)</a></p>
      </section>
    </main>
  `
})
export class LandingComponent {
  private router = inject(Router);
  private auth = inject(AuthService);

  enterAdmin(event: Event, path: string) {
    event.preventDefault();
    // mark current session as admin for admin flows
    try {
      this.auth.becomeAdmin();
    } catch {
      // noop — tests may stub AuthService
    }
    this.router.navigate([path]);
  }
}
