import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:5001/',
  redirectUri: baseUrl,
  clientId: 'DuanEcommerce_Web',
  responseType: 'code',
  scope: 'offline_access DuanEcommerce.Admin',
  requireHttps: true,
  dummyClientSecret: '1q2w3e*',
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'DuanEcommerce',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:5001',
      rootNamespace: 'DuanEcommerce.Admin',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
