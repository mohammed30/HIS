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
      path: '/setting-management',
      name: '::Menu:Settings',
      iconClass: 'fas fa-cog',
      order: 300,
      layout: eLayoutType.application,
    },
  ]);
}
