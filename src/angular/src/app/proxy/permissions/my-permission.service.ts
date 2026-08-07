import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { GetPermissionListResultDto, GetResourcePermissionDefinitionListResultDto, GetResourcePermissionListResultDto, GetResourcePermissionWithProviderListResultDto, GetResourceProviderListResultDto, SearchProviderKeyListResultDto, UpdatePermissionsDto, UpdateResourcePermissionsDto } from '../volo/abp/permission-management/models';

@Injectable({
  providedIn: 'root',
})
export class MyPermissionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  deleteResource = (resourceName: string, resourceKey: string, providerName: string, providerKey: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/my-permission/resource',
      params: { resourceName, resourceKey, providerName, providerKey },
    },
    { apiName: this.apiName,...config });
  

  get = (providerName: string, providerKey: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetPermissionListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission',
      params: { providerName, providerKey },
    },
    { apiName: this.apiName,...config });
  

  getByGroup = (groupName: string, providerName: string, providerKey: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetPermissionListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission/by-group',
      params: { groupName, providerName, providerKey },
    },
    { apiName: this.apiName,...config });
  

  getResource = (resourceName: string, resourceKey: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetResourcePermissionListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission/resource',
      params: { resourceName, resourceKey },
    },
    { apiName: this.apiName,...config });
  

  getResourceByProvider = (resourceName: string, resourceKey: string, providerName: string, providerKey: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetResourcePermissionWithProviderListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission/resource-by-provider',
      params: { resourceName, resourceKey, providerName, providerKey },
    },
    { apiName: this.apiName,...config });
  

  getResourceDefinitions = (resourceName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetResourcePermissionDefinitionListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission/resource-definitions',
      params: { resourceName },
    },
    { apiName: this.apiName,...config });
  

  getResourceProviderKeyLookupServices = (resourceName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GetResourceProviderListResultDto>({
      method: 'GET',
      url: '/api/app/my-permission/resource-provider-key-lookup-services',
      params: { resourceName },
    },
    { apiName: this.apiName,...config });
  

  searchResourceProviderKey = (resourceName: string, serviceName: string, filter: string, page: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SearchProviderKeyListResultDto>({
      method: 'POST',
      url: '/api/app/my-permission/search-resource-provider-key',
      params: { resourceName, serviceName, filter, page },
    },
    { apiName: this.apiName,...config });
  

  update = (providerName: string, providerKey: string, input: UpdatePermissionsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/my-permission',
      params: { providerName, providerKey },
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateResource = (resourceName: string, resourceKey: string, input: UpdateResourcePermissionsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/my-permission/resource',
      params: { resourceName, resourceKey },
      body: input,
    },
    { apiName: this.apiName,...config });
}