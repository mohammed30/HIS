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
    path: 'patients/services-report',
    loadComponent: () => import('./patients/patient-services-report/patient-services-report').then(c => c.PatientServicesReport),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Patients' }
  },
  {
    path: 'patients/:id/services-report',
    loadComponent: () => import('./patients/patient-services-report/patient-services-report').then(c => c.PatientServicesReport),
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
    canActivate: [authGuard],
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
      },
      {
        path: 'job-titles',
        loadComponent: () => import('./settings/job-titles/job-titles.component').then(c => c.JobTitlesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      },
      {
        path: 'pharmacy',
        loadComponent: () => import('./settings/pharmacy/pharmacy-settings.component').then(c => c.PharmacySettingsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      }
    ]
  },
  {
    path: 'definitions',
    canActivate: [authGuard],
    children: [
      {
        path: 'nationalities',
        loadComponent: () => import('./definitions/nationalities/nationalities.component').then(c => c.NationalitiesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.Nationalities' }
      },
      {
        path: 'payment-methods',
        loadComponent: () => import('./definitions/payment-methods/payment-methods.component').then(c => c.PaymentMethodsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.PaymentMethods' }
      },
      {
        path: 'professions',
        loadComponent: () => import('./definitions/professions/professions.component').then(c => c.ProfessionsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.Professions' }
      },
      {
        path: 'contracts',
        loadComponent: () => import('./definitions/contracts/contracts.component').then(c => c.ContractsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.Contracts' }
      },
      {
        path: 'patient-categories',
        loadComponent: () => import('./definitions/patient-categories/patient-categories.component').then(c => c.PatientCategoriesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.PatientCategories' }
      },
      {
        path: 'referral-sources',
        loadComponent: () => import('./definitions/referral-sources/referral-sources.component').then(c => c.ReferralSourcesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Definitions.ReferralSources' }
      }
    ]
  },
  {
    path: 'appointments',
    loadChildren: () => import('./appointments/appointments-module').then(m => m.AppointmentsModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Appointments' }
  },
  {
    path: 'financials',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Billing' },
    children: [
      {
        path: 'chart-of-accounts',
        loadComponent: () => import('./financials/chart-of-accounts/chart-of-accounts.component').then(c => c.ChartOfAccountsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.ChartOfAccounts' }
      }
    ]
  },
  {
    path: 'reception',
    canActivate: [authGuard],
    children: [
      {
        path: 'insurance-companies',
        loadComponent: () => import('./reception/insurance/insurance-companies.component').then(c => c.InsuranceCompaniesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Reception.InsuranceCompanies' }
      },
      {
        path: 'insurance-plans',
        loadComponent: () => import('./reception/insurance/insurance-plans.component').then(c => c.InsurancePlansComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Reception.InsurancePlans' }
      },
      {
        path: 'invoices',
        loadComponent: () => import('./reception/billing/invoices.component').then(c => c.InvoicesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Reception.Invoices' }
      },
      {
        path: 'payments',
        loadComponent: () => import('./reception/billing/payments.component').then(c => c.PaymentsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Reception.Payments' }
      },
      {
        path: 'deferred-payments',
        loadComponent: () => import('./reception/billing/deferred-payments.component').then(c => c.DeferredPaymentsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.DeferredPayments' }
      },
      {
        path: 'laboratory-reception',
        loadComponent: () => import('./reception/lab-reception/laboratory-reception.component').then(c => c.LaboratoryReceptionComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Reception.Tickets' }
      }
    ]
  },
  {
    path: 'inpatient',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./inpatient/room-dashboard/room-dashboard.component').then(c => c.RoomDashboardComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inpatient.Dashboard' }
      },
      {
        path: 'admissions',
        loadComponent: () => import('./inpatient/admission-list/admission-list.component').then(c => c.AdmissionListComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inpatient.Admissions' }
      },
      {
        path: 'rooms',
        loadChildren: () => import('./inpatient/rooms/rooms-module').then(m => m.RoomsModule),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inpatient.Rooms' }
      },
      {
        path: 'reservations',
        loadChildren: () => import('./inpatient/reservations/reservations-module').then(m => m.ReservationsModule),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inpatient.Reservations' }
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
        loadComponent: () => import('./admin/activity-logs/activity-logs.component').then(c => c.ActivityLogsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Settings' }
      }
    ]
  },
  {
    path: 'accounting',
    canActivate: [authGuard],
    children: [
      {
        path: 'chart-of-accounts',
        loadComponent: () => import('./accounting/chart-of-accounts/chart-of-accounts.component').then(c => c.ChartOfAccountsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.ChartOfAccounts' }
      },
      {
        path: 'journal-entries',
        loadComponent: () => import('./accounting/journal-entries/journal-entries').then(c => c.JournalEntriesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.JournalEntries' }
      },
      {
        path: 'payment-vouchers',
        loadComponent: () => import('./accounting/payment-vouchers/payment-vouchers/payment-vouchers').then(c => c.PaymentVouchers),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.PaymentVouchers' }
      },
      {
        path: 'receipt-vouchers',
        loadComponent: () => import('./accounting/receipt-vouchers/receipt-vouchers/receipt-vouchers').then(c => c.ReceiptVouchers),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.ReceiptVouchers' }
      },
      {
        path: 'bank-transactions',
        loadComponent: () => import('./accounting/bank-transactions/bank-transactions/bank-transactions').then(c => c.BankTransactions),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.BankTransactions' }
      },
      {
        path: 'claims',
        loadComponent: () => import('./accounting/claims/contract-claims/contract-claims').then(c => c.ContractClaims),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.ContractClaims' }
      },
      {
        path: 'reports/daily',
        loadComponent: () => import('./accounting/reports/daily-accounts-report/daily-accounts-report').then(c => c.DailyAccountsReport),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports/debts',
        loadComponent: () => import('./accounting/reports/customer-debts-report/customer-debts-report').then(c => c.CustomerDebtsReport),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports/income-statement',
        loadComponent: () => import('./accounting/reports/income-statement/income-statement').then(c => c.IncomeStatementComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports/balance-sheet',
        loadComponent: () => import('./accounting/reports/balance-sheet/balance-sheet').then(c => c.BalanceSheetComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports/discounts',
        loadComponent: () => import('./accounting/reports/discounts-report/discounts-report').then(c => c.DiscountsReport),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports/account-statement',
        loadComponent: () => import('./accounting/reports/account-statement/account-statement.component').then(c => c.AccountStatementComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      },
      {
        path: 'reports',
        loadComponent: () => import('./accounting/reports/financial-reports.component').then(c => c.FinancialReportsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Billing.FinancialReports' }
      }
    ]
  },
  {
    path: 'inventory',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./inventory/dashboard/inventory-dashboard.component').then(c => c.InventoryDashboardComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.Dashboard' }
      },
      {
        path: 'warehouse-management',
        loadComponent: () => import('./inventory/warehouse-management/warehouse-management.component').then(c => c.WarehouseManagementComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.ManageWarehouses' }
      },
      {
        path: 'item-card/:id',
        loadComponent: () => import('./inventory/item-card/item-card.component').then(c => c.ItemCardComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory' }
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
      },
      {
        path: 'internal-requests',
        loadComponent: () => import('./inventory/internal-requests/internal-requests').then(c => c.InternalRequestsComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.StockOperations' }
      },
      {
        path: 'purchase-invoices',
        loadComponent: () => import('./inventory/purchase-invoices/purchase-invoices.component').then(c => c.PurchaseInvoicesComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.PurchaseOrders' }
      },
      {
        path: 'suppliers',
        loadChildren: () => import('./inventory/suppliers/suppliers-module').then(m => m.SuppliersModule),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.Suppliers' }
      },
      {
        path: 'purchase-orders',
        loadChildren: () => import('./inventory/purchase-orders/purchase-orders.module').then(m => m.PurchaseOrdersModule),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.PurchaseOrders' }
      },
      {
        path: 'purchase-requisitions',
        loadChildren: () => import('./inventory/purchase-requisitions/purchase-requisitions-routing.module').then(m => m.PurchaseRequisitionsRoutingModule),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.PurchaseRequisitions' }
      },
      {
        path: 'reports/department-consumption',
        loadComponent: () => import('./inventory/reports/department-consumption-report.component').then(c => c.DepartmentConsumptionReportComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.DepartmentalConsumption' }
      },
      {
        path: 'reports/low-stock',
        loadComponent: () => import('./inventory/reports/low-stock-report.component').then(c => c.LowStockReportComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.StockOperations' }
      },
      {
        path: 'reports/stagnant-stock',
        loadComponent: () => import('./inventory/reports/stagnant-stock-report.component').then(c => c.StagnantStockReportComponent),
        canActivate: [permissionGuard],
        data: { requiredPolicy: 'HIS.Inventory.StockOperations' }
      }
    ]
  },
  {
    path: 'laboratory',
    loadChildren: () => import('./laboratory/lab-module').then(m => m.LabModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Laboratory' }
  },
  {
    path: 'emergency',
    loadChildren: () => import('./emergency/emergency-module').then(m => m.EmergencyModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Emergency' }
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
  {
    path: 'nursing',
    loadChildren: () => import('./nursing/nursing-module').then(m => m.NursingModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Nursing' }
  },
  {
    path: 'hr',
    loadChildren: () => import('./hr/hr-module').then(m => m.HrModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.HR' }
  },
  {
    path: 'operations/surgical-operations',
    loadComponent: () => import('./operations/surgical-operations/surgical-operations').then(c => c.SurgicalOperations),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Operations' }
  },
  {
    path: 'operations/reports/surgical-operations',
    loadComponent: () => import('./operations/reports/operations-report.component').then(c => c.OperationsReportComponent),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Operations.Report' }
  },
  {
    path: 'reports',
    loadChildren: () => import('./reports/reports.module').then(m => m.ReportsModule),
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: 'HIS.Operations.Report' }
  },
];
