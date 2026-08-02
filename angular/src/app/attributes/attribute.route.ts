import { Routes } from '@angular/router';
import { Attributes } from './attributes';

export default [
  { path: '', component: Attributes },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
