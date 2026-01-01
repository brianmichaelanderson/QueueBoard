import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-admin-list',
  imports: [CommonModule],
  template: `
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Admin</h1>
        <p>Placeholder for 'AdminListComponent'. Add admin UI here.</p>
      </main>
    </div>
  `
})
export class AdminListComponent {}
