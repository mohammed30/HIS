import { Environment } from '@abp/ng.core';

const baseUrl = 'http://asiahospitalhis-001-site1.qtempurl.com';
const oAuthConfig = {
  issuer: 'http://strategicmoves-001-site1.jtempurl.com',
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
      url: 'http://strategicmoves-001-site1.jtempurl.com/',
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
