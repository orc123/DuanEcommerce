import { Routes } from '@angular/router';
import { Roles } from './roles';

export default [
  { path: '', component: Roles },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
