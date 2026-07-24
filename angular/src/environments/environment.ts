import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44351/',
  redirectUri: baseUrl,
  clientId: 'DuanEcommerce_App',
  responseType: 'code',
  scope: 'offline_access DuanEcommerce',
  requireHttps: true,
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
      url: 'https://localhost:44351',
      rootNamespace: 'DuanEcommerce',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
