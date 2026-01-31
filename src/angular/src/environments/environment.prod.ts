import { Environment } from '@abp/ng.core';

const baseUrl = 'https://asiahospitalfront-dkbhcbftfcg5g4f7.westeurope-01.azurewebsites.net';

const oAuthConfig = {
  issuer: 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net/',
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
    name: 'Asia Hospital',
    logoUrl: 'assets/images/logo/icon-lite.svg',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://asiahospitalback-ffdkapgqauaherbd.westeurope-01.azurewebsites.net',
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
