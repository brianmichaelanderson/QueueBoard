import { Component } from '@angular/core';
import { AdminDetailComponent } from './admin-detail.component';

@Component({
  standalone: true,
  selector: 'app-admin-detail-wrapper',
  imports: [AdminDetailComponent],
  template: `
    <!-- Thin wrapper that reuses AdminDetailComponent and signals admin edit UI via route.data -->
    <app-admin-detail></app-admin-detail>
  `
})
export class AdminDetailWrapperComponent {}
