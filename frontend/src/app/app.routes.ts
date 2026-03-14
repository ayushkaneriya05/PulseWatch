import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'api-management',
    loadComponent: () => import('./features/api-management/api-list/api-list.component').then(m => m.ApiListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'api-management/new',
    loadComponent: () => import('./features/api-management/api-form/api-form.component').then(m => m.ApiFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'api-management/edit/:id',
    loadComponent: () => import('./features/api-management/api-form/api-form.component').then(m => m.ApiFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'api-detail/:id',
    loadComponent: () => import('./features/api-detail/api-detail.component').then(m => m.ApiDetailComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];
