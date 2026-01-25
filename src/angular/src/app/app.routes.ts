import { authGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
    canActivate: [authGuard],
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
    canActivate: [authGuard],
  },
  {
    path: 'patients',
    loadComponent: () => import('./patients/patients.component').then(c => c.PatientsComponent),
    canActivate: [authGuard],
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
    canActivate: [authGuard],
  },
  {
    path: 'settings',
    canActivate: [authGuard],
    children: [
      {
        path: 'departments',
        loadComponent: () => import('./settings/departments/departments.component').then(c => c.DepartmentsComponent)
      },
      {
        path: 'specialties',
        loadComponent: () => import('./settings/specialties/specialties.component').then(c => c.SpecialtiesComponent)
      },
      {
        path: 'clinics',
        loadComponent: () => import('./settings/clinics/clinics.component').then(c => c.ClinicsComponent)
      },
      {
        path: 'doctors',
        loadComponent: () => import('./settings/doctors/doctors.component').then(c => c.DoctorsComponent)
      },
      {
        path: 'laboratories',
        loadComponent: () => import('./settings/laboratories/laboratories.component').then(c => c.LaboratoriesComponent)
      }
    ]
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
    canActivate: [authGuard],
  },
];
