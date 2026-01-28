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
    path: 'services',
    loadChildren: () => import('./services/services-module').then(m => m.ServicesModule),
    canActivate: [authGuard],
  },
  {
    path: 'patients',
    loadComponent: () => import('./patients/patients.component').then(c => c.PatientsComponent),
    canActivate: [authGuard],
  },
  {
    path: 'patients/:id/medical-record',
    loadComponent: () => import('./patients/medical-record/patient-medical-record.component').then(c => c.PatientMedicalRecordComponent),
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
        path: 'hospital',
        loadComponent: () => import('./settings/hospital/hospital-settings.component').then(c => c.HospitalSettingsComponent)
      },
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
      },
      {
        path: 'doctor-schedule',
        loadComponent: () => import('./settings/doctor-schedule').then(c => c.DoctorScheduleComponent)
      }
    ]
  },
  {
    path: 'appointments',
    canActivate: [authGuard],
    children: [
      {
        path: 'booking',
        loadComponent: () => import('./appointments/booking/booking').then(c => c.BookingComponent)
      },
      {
        path: 'my-appointments',
        loadComponent: () => import('./appointments/my-appointments/my-appointments').then(c => c.MyAppointmentsComponent)
      },
      {
        path: 'waiting-list',
        loadComponent: () => import('./appointments/waiting-list/waiting-list').then(c => c.WaitingListComponent)
      },
      {
        path: 'doctor-schedule',
        loadComponent: () => import('./appointments/doctor-schedule/doctor-schedule').then(c => c.DoctorScheduleComponent)
      }
    ]
  },
  {
    path: 'financials',
    canActivate: [authGuard],
    children: [
      {
        path: 'chart-of-accounts',
        loadComponent: () => import('./financials/chart-of-accounts/chart-of-accounts.component').then(c => c.ChartOfAccountsComponent)
      }
    ]
  },
  {
    path: 'reception',
    canActivate: [authGuard],
    children: [
      {
        path: 'insurance-companies',
        loadComponent: () => import('./reception/insurance/insurance-companies.component').then(c => c.InsuranceCompaniesComponent)
      },
      {
        path: 'insurance-plans',
        loadComponent: () => import('./reception/insurance/insurance-plans.component').then(c => c.InsurancePlansComponent)
      },
      {
        path: 'invoices',
        loadComponent: () => import('./reception/billing/invoices.component').then(c => c.InvoicesComponent)
      },
      {
        path: 'payments',
        loadComponent: () => import('./reception/billing/payments.component').then(c => c.PaymentsComponent)
      },
      {
        path: 'deferred-payments',
        loadComponent: () => import('./reception/billing/deferred-payments.component').then(c => c.DeferredPaymentsComponent)
      }
    ]
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    children: [
      {
        path: 'activity-logs',
        loadComponent: () => import('./admin/activity-logs/activity-logs.component').then(c => c.ActivityLogsComponent)
      }
    ]
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
    canActivate: [authGuard],
  },
];
