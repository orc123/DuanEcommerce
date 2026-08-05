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
        path: 'products',
        loadChildren: () => import('./products/product.route'),
      },
      {
        path: 'attributes',
        loadChildren: () => import('./attributes/attribute.route'),
      },
      {
        path: 'roles',
        loadChildren: () => import('./roles/role.route'),
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
  { path: 'auth', loadChildren: () => import('../app/auth/auth.routes') },
];
