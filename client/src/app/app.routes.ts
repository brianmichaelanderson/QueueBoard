import { Routes } from '@angular/router';


export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./landing/landing.component').then(m => m.LandingComponent)
  },
	{
		path: 'queues',
		loadChildren: () => import('./queues/queues.routes').then(m => m.queuesRoutes)
	},
	{
		path: 'agents',
		loadChildren: () => import('./agent').then(m => m.agentRoutes)
	},
	{
		path: 'admin',
		loadChildren: () => import('./admin').then(m => m.adminRoutes)
	},
	{
		path: '**',
		loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
	}
];
