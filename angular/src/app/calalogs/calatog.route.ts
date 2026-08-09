import { Routes } from '@angular/router';
import { Products } from './products/products';
import { Attributes } from './attributes/attributes';
import { permissionGuard, PermissionGuard } from '@abp/ng.core';

export default [
  {
    path: 'products',
    component: Products,
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'DuanEcomAdminCatalog.Product',
    },
  },
  {
    path: 'attributes',
    component: Attributes,
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'DuanEcomAdminCatalog.Attribute',
    },
  },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
