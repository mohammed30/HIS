import { Environment } from '@abp/ng.core';

const baseUrl = 'https://asiahosp-001-site1.htempurl.com';
const oAuthConfig = {
  issuer: 'https://asiaback2000-001-site1.gtempurl.com',
  redirectUri: baseUrl,
  clientId: 'HIS_App',
  responseType: 'code',
  scope: 'offline_access HIS',
  requireHttps: false,
  skipIssuerCheck: true,
  strictDiscoveryDocumentValidation: false,
  clearHashAfterLogin: false,
  silentRefreshRedirectUri: baseUrl,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Asia Hospital',
    logoUrl: 'assets/images/logo/Dark.png',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://asiaback2000-001-site1.gtempurl.com',
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
