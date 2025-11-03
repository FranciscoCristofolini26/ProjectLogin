import { Routes } from '@angular/router';
import { Signup } from './signup/signup';

export const routes: Routes = [
  { path: '', redirectTo: 'signup', pathMatch: 'full' },
  { path: 'signup', component: Signup },
];
