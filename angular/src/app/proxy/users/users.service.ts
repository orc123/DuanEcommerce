import type { CreateUserDto, SetPasswordDto, UpdateUserDto, UserDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { BaseListFilterDto } from '../models';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  assignRole = (userId: string, roleNames: string[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/users/assign-role/${userId}`,
      body: roleNames,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUserDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserDto>({
      method: 'POST',
      url: '/api/app/users',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/users/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteMultiple = (ids: string[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/users/multiple',
      params: { ids },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserDto>({
      method: 'GET',
      url: `/api/app/users/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UserDto>>({
      method: 'GET',
      url: '/api/app/users',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getListAll = (filterKeyword: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserDto[]>({
      method: 'GET',
      url: '/api/app/users/all',
      params: { filterKeyword },
    },
    { apiName: this.apiName,...config });
  

  getListWithFilter = (input: BaseListFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UserDto>>({
      method: 'GET',
      url: '/api/app/users/with-filter',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  setPassword = (userId: string, input: SetPasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/users/set-password/${userId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateUserDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserDto>({
      method: 'PUT',
      url: `/api/app/users/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}