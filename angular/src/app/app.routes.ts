import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { AppLayout } from './layout/component/app.layout';

export const APP_ROUTES: Routes = [
  {
    path: '',
    component: AppLayout,
    children: [
      {
        path: '',
        loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
      },
      {
        path: 'identity',
        loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
      },
      {
        path: 'tenant-management',
        loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
      },
      {
        path: 'setting-management',
        loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
      },
    ],
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
];

