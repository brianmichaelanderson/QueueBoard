import { Routes } from '@angular/router';

export const routes: Routes = [
	{ path: '', redirectTo: 'queues', pathMatch: 'full' },
	{
		path: 'queues',
		loadComponent: () => import('./queues/queues.component').then(m => m.QueuesComponent)
	},
	{
		path: '**',
		loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
	}
];
