import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    // Home
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    // Patients
    {
      path: '/patients',
      name: '::Menu:Patients',
      iconClass: 'fas fa-user-injured',
      order: 10,
      layout: eLayoutType.application,
    },
    // Appointments
    {
      name: '::Menu:Appointments',
      iconClass: 'fas fa-calendar-check',
      order: 20,
      layout: eLayoutType.application,
    },
    {
      path: '/appointments/booking',
      name: '::Menu:BookNewAppointment',
      parentName: '::Menu:Appointments',
      iconClass: 'fas fa-calendar-plus',
      order: 21,
      layout: eLayoutType.application,
    },
    {
      path: '/appointments/my-appointments',
      name: '::Menu:MyAppointments',
      parentName: '::Menu:Appointments',
      iconClass: 'fas fa-list-alt',
      order: 22,
      layout: eLayoutType.application,
    },
    // Accounts (Financials)
    {
      name: '::Menu:Accounts',
      iconClass: 'fas fa-file-invoice-dollar',
      order: 30,
      layout: eLayoutType.application,
    },
    {
      path: '/accounting/chart-of-accounts',
      name: '::Menu:ChartOfAccounts',
      parentName: '::Menu:Accounts',
      iconClass: 'fas fa-sitemap',
      order: 31,
      layout: eLayoutType.application,
    },
    {
      path: '/accounting/journal-entries',
      name: '::Menu:JournalEntries',
      parentName: '::Menu:Accounts',
      iconClass: 'fas fa-book',
      order: 32,
      layout: eLayoutType.application,
    },
    // Insurance
    {
      name: '::Menu:Insurance',
      iconClass: 'fas fa-shield-alt',
      order: 40,
      layout: eLayoutType.application,
    },
    {
      path: '/reception/insurance-companies',
      name: '::Menu:InsuranceCompanies',
      parentName: '::Menu:Insurance',
      iconClass: 'fas fa-building',
      order: 41,
      layout: eLayoutType.application,
    },
    {
      path: '/reception/insurance-plans',
      name: '::Menu:InsurancePlans',
      parentName: '::Menu:Insurance',
      iconClass: 'fas fa-file-contract',
      order: 42,
      layout: eLayoutType.application,
    },
    // Billing
    {
      name: '::Menu:Billing',
      iconClass: 'fas fa-money-bill-wave',
      order: 50,
      layout: eLayoutType.application,
    },
    {
      path: '/reception/invoices',
      name: '::Menu:Invoices',
      parentName: '::Menu:Billing',
      iconClass: 'fas fa-file-invoice',
      order: 51,
      layout: eLayoutType.application,
    },
    {
      path: '/reception/payments',
      name: '::Menu:Payments',
      parentName: '::Menu:Billing',
      iconClass: 'fas fa-cash-register',
      order: 52,
      layout: eLayoutType.application,
    },
    {
      path: '/reception/deferred-payments',
      name: '::Menu:DeferredPayments',
      parentName: '::Menu:Billing',
      iconClass: 'fas fa-clock',
      order: 53,
      layout: eLayoutType.application,
    },
    // Definitions
    {
      name: '::Menu:Definitions',
      iconClass: 'fas fa-cogs',
      order: 100,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/departments',
      name: '::Menu:Departments',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-building',
      order: 101,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/specialties',
      name: '::Menu:Specialties',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-stethoscope',
      order: 102,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/clinics',
      name: '::Menu:Clinics',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-clinic-medical',
      order: 103,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/doctors',
      name: '::Menu:Doctors',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-user-md',
      order: 104,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/laboratories',
      name: '::Menu:Laboratories',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-flask',
      order: 105,
      layout: eLayoutType.application,
    },
    {
      path: '/settings/doctor-schedule',
      name: '::Menu:DoctorSchedule',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-clock',
      order: 106,
      layout: eLayoutType.application,
    },
    {
      path: '/appointments/waiting-list',
      name: '::Menu:WaitingList',
      parentName: '::Menu:Appointments',
      iconClass: 'fas fa-list-ol',
      order: 23,
      layout: eLayoutType.application,
    },
    // Services & Pricing
    {
      path: '/services',
      name: '::Menu:Services',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-briefcase-medical',
      order: 107,
      layout: eLayoutType.application,
    },
    {
      path: '/services/radiology',
      name: '::Menu:Radiology',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-x-ray',
      order: 108,
      layout: eLayoutType.application,
    },
    {
      path: '/services/price-lists',
      name: '::Menu:PriceLists',
      parentName: '::Menu:Definitions',
      iconClass: 'fas fa-tags',
      order: 109,
      layout: eLayoutType.application,
    },
    // Administration
    {
      path: '/identity',
      name: '::Menu:UserManagement',
      iconClass: 'fas fa-users-cog',
      order: 200,
      layout: eLayoutType.application,
      requiredPolicy: 'AbpIdentity.Users',
    },
    {
      path: '/admin/activity-logs',
      name: '::Menu:ActivityLogs',
      iconClass: 'fas fa-history',
      order: 210,
      layout: eLayoutType.application,
    },
    // Inventory
    {
      name: '::Menu:Inventory',
      iconClass: 'fas fa-boxes',
      order: 35,
      layout: eLayoutType.application,
    },
    {
      path: '/inventory/dashboard',
      name: '::Menu:InventoryDashboard',
      parentName: '::Menu:Inventory',
      iconClass: 'fas fa-tachometer-alt',
      order: 36,
      layout: eLayoutType.application,
    },
    {
      path: '/inventory/warehouse-management',
      name: '::Menu:WarehouseManagement',
      parentName: '::Menu:Inventory',
      iconClass: 'fas fa-warehouse',
      order: 37,
      layout: eLayoutType.application,
    },
    {
      path: '/setting-management',
      name: '::Menu:Settings',
      iconClass: 'fas fa-cog',
      order: 300,
      layout: eLayoutType.application,
    },
  ]);
}
