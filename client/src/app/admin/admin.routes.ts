import { Routes } from '@angular/router';
import { adminGuard } from './admin.guard';
import { authGuard } from '../auth/auth.guard';
import { AdminService } from '../services/admin.service';

/**
 * Admin feature routes (wrapper pattern)
 *
 * Pattern summary:
 * - The admin module provides admin-only flows while reusing shared feature components
 *   (agents/queues) to avoid template duplication.
 * - Thin wrapper components (e.g. `AdminAgentListComponent`, `AdminAgentEditComponent`)
 *   render the corresponding agent components and exist primarily to set route-level
 *   metadata and providers for admin scenarios.
 *
 * Route-data conventions used here:
 * - `roles: ['admin']` — indicates the route requires the admin role (checked by guards).
 * - `showEditButtons: true` — hint for wrappers/shared components to show admin-only
 *   edit/create UI (wrappers set this flag on admin edit/create routes).
 */
export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-agent-list.component').then(m => m.AdminAgentListComponent),
    canActivate: [adminGuard, authGuard],
    // admin list is guarded; wrapper reuses AgentListComponent
    data: { roles: ['admin'] },
    providers: [AdminService]
  },
  {
    path: 'create',
    loadComponent: () => import('./admin-agent-edit.component').then(m => m.AdminAgentEditComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-agent-detail.component').then(m => m.AdminAgentDetailComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  }
];
