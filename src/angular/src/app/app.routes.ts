import { authGuard, permissionGuard } from '@abp/ng.core';
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
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Settings' }
  },
  {
    path: 'patients',
    loadComponent: () => import('./patients/patients.component').then(c => c.PatientsComponent),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Patients' }
  },
  {
    path: 'patients/:id/medical-record',
    loadComponent: () => import('./patients/medical-record/patient-medical-record.component').then(c => c.PatientMedicalRecordComponent),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Patients' }
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'AbpIdentity.Users' }
  },
  {
    path: 'settings',
    canActivate: [authGuard, permissionGuard],
    // data: { requiredPolicy: 'HIS.Settings' }, // Removed to allow granular child control
    children: [
      {
        path: 'hospital',
        loadComponent: () => import('./settings/hospital/hospital-settings.component').then(c => c.HospitalSettingsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'departments',
        loadComponent: () => import('./settings/departments/departments.component').then(c => c.DepartmentsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'specialties',
        loadComponent: () => import('./settings/specialties/specialties.component').then(c => c.SpecialtiesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'clinics',
        loadComponent: () => import('./settings/clinics/clinics.component').then(c => c.ClinicsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'doctors',
        loadComponent: () => import('./settings/doctors/doctors.component').then(c => c.DoctorsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'laboratories',
        loadComponent: () => import('./settings/laboratories/laboratories.component').then(c => c.LaboratoriesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Laboratory' }
      },
      {
        path: 'doctor-schedule',
        loadComponent: () => import('./settings/doctor-schedule').then(c => c.DoctorScheduleComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      }
    ]
  },
  {
    path: 'appointments',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Appointments' },
    children: [
      {
        path: 'booking',
        loadComponent: () => import('./appointments/booking/booking').then(c => c.BookingComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Appointments.Create' }
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
      },
      {
        path: 'flow',
        loadComponent: () => import('./appointments/clinic-flow/clinic-flow.component').then(c => c.ClinicFlowComponent)
      }
    ]
  },
  {
    path: 'financials',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Billing' },
    children: [
      {
        path: 'chart-of-accounts',
        loadComponent: () => import('./financials/chart-of-accounts/chart-of-accounts.component').then(c => c.ChartOfAccountsComponent)
      }
    ]
  },
  {
    path: 'reception',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Billing' },
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
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Settings' },
    children: [
      {
        path: 'activity-logs',
        loadComponent: () => import('./admin/activity-logs/activity-logs.component').then(c => c.ActivityLogsComponent)
      }
    ]
  },
  {
    path: 'accounting',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Billing' },
    children: [
      {
        path: 'chart-of-accounts',
        loadComponent: () => import('./accounting/chart-of-accounts/chart-of-accounts.component').then(c => c.ChartOfAccountsComponent)
      },
      {
        path: 'journal-entries',
        loadComponent: () => import('./accounting/journal-entries/journal-entries').then(c => c.JournalEntriesComponent)
      }
    ]
  },
  {
    path: 'inventory',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Inventory' },
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./inventory/dashboard/inventory-dashboard.component').then(c => c.InventoryDashboardComponent)
      },
      {
        path: 'warehouse-management',
        loadComponent: () => import('./inventory/warehouse-management/warehouse-management.component').then(c => c.WarehouseManagementComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.ManageWarehouses' }
      },
      {
        path: 'item-card/:id',
        loadComponent: () => import('./inventory/item-card/item-card.component').then(c => c.ItemCardComponent)
      },
      {
        path: 'receive-stock',
        loadComponent: () => import('./inventory/receive-stock/receive-stock.component').then(c => c.ReceiveStockComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.StockOperations' }
      },
      {
        path: 'issue-stock',
        loadComponent: () => import('./inventory/issue-stock/issue-stock.component').then(c => c.IssueStockComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.StockOperations' }
      }
    ]
  },
  {
    path: 'laboratory',
    loadChildren: () => import('./laboratory/lab-module').then(m => m.LabModule),
    canActivate: [authGuard],
  },
  {
    path: 'emergency',
    loadChildren: () => import('./emergency/emergency-module').then(m => m.EmergencyModule),
    canActivate: [authGuard],
  },
  {
    path: 'pharmacy',
    loadChildren: () => import('./pharmacy/pharmacy.module').then(m => m.PharmacyModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Pharmacy' }
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Settings' }
  },
];
// Force rebuild
