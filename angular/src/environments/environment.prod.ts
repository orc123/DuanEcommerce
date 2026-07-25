import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:5001/',
  redirectUri: baseUrl,
  clientId: 'DuanEcommerce_Web',
  responseType: 'code',
  scope: 'offline_access DuanEcommerce',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'DuanEcommerce',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:5001',
      rootNamespace: 'DuanEcommerce',
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
