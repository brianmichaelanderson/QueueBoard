import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-not-found',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">404 — Not Found</h1>
        <p>The page you requested could not be found.</p>
      </main>
    </div>
  `,
  styles: [
    `:host { display: block; }`,
    `.page-title { margin-bottom: 0.5rem; }`
  ]
})
export class NotFoundComponent {}
