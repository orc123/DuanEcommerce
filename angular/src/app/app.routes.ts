import { Routes } from '@angular/router';
import { AppLayout } from './layout/component/app.layout';
import { authGuard } from './shared/guards/auth.guard';

export const APP_ROUTES: Routes = [
  {
    path: '',
    component: AppLayout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
      },
      {
        path: 'calatogs',
        loadChildren: () => import('./calalogs/calatog.route'),
      },
      {
        path: 'systems',
        loadChildren: () => import('./systems/system.route'),
      },
      // {
      //   path: 'tenant-management',
      //   loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
      // },
      // {
      //   path: 'setting-management',
      //   loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
      // },
    ],
  },
  { path: 'auth', loadChildren: () => import('./auth/auth.routes') },
];
