import { Component } from '@angular/core';
import { AdminEditComponent } from './admin-edit.component';

@Component({
  standalone: true,
  selector: 'app-admin-edit-wrapper',
  imports: [AdminEditComponent],
  template: `
    <!-- Thin wrapper that reuses AdminEditComponent and signals admin edit UI via route.data -->
    <app-admin-edit></app-admin-edit>
  `
})
export class AdminEditWrapperComponent {}
