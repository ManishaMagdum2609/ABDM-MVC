import { Routes } from '@angular/router';
import { ABHA } from './Core/Features/Components/abha-Login/abhaLogin-component';

export const routes: Routes = [
  { path: 'abha', component:  ABHA  },
  { path: '', redirectTo: 'abha', pathMatch: 'full' }
];
