import { Routes } from '@angular/router';
import { Product } from './product';

export default [
  { path: '', component: Product },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
