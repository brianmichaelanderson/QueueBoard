import { Routes } from '@angular/router';

export const routes: Routes = [
	{ path: '', redirectTo: 'queues', pathMatch: 'full' },
	{
		path: 'queues',
		loadChildren: () => import('./queues/queues.routes').then(m => m.queuesRoutes)
	},
	{
		path: '**',
		loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
	}
];
