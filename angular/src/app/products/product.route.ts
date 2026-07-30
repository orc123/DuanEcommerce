import { Routes } from '@angular/router';
import { Products } from './products';

export default [
  { path: '', component: Products },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
