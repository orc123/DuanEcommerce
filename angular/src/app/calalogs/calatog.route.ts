import { Routes } from '@angular/router';
import { Products } from './products/products';
import { Attributes } from './attributes/attributes';

export default [
  { path: 'products', component: Products },
  { path: 'attributes', component: Attributes },
  { path: '**', redirectTo: 'auth/error' },
] as Routes;
