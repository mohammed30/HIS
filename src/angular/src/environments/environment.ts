import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44382/',
  redirectUri: baseUrl,
  clientId: 'HIS_App',
  responseType: 'code',
  scope: 'offline_access HIS',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'AppName',
    logoUrl: 'assets/images/logo/icon-lite.svg',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44382',
      rootNamespace: 'HIS',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  localization: {
    defaultResourceName: 'HIS',
    defaultCulture: 'ar',
  },
} as Environment;
