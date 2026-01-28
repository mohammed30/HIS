import { Environment } from '@abp/ng.core';

const baseUrl = 'https://asia.tryasp.net';

const oAuthConfig = {
  issuer: 'https://asia.runasp.net/',
  redirectUri: baseUrl,
  clientId: 'HIS_App',
  responseType: 'code',
  scope: 'offline_access HIS',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'AppName',
    logoUrl: 'assets/images/logo/icon-lite.svg',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://asia.runasp.net',
      rootNamespace: 'HIS',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge',
  },
} as Environment;
