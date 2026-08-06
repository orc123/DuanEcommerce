import { Routes } from '@angular/router';
import { Roles } from './roles/roles';
import { Users } from './users/users';

export default [
  { path: 'roles', component: Roles },
  { path: 'users', component: Users },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
