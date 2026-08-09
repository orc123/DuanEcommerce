import { Routes } from '@angular/router';
import { Roles } from './roles/roles';
import { Users } from './users/users';
import { PermissionGrant } from './roles/permission-grant';
import { permissionGuard } from '@abp/ng.core';

export default [
  {
    path: 'roles',
    component: Roles,
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'AbpIdentity.Roles',
    },
  },
  {
    path: 'users',
    component: Users,
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'AbpIdentity.Users',
    },
  },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
