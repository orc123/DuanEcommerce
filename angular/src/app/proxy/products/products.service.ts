import type { AddUpdateProductAttributeDto, ProductAttributeListFilterDto, ProductAttributeValueDto } from './attributes/models';
import type { CreateUpdateProductDto, ProductDto, ProductListFilterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addAttribute = (input: AddUpdateProductAttributeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeValueDto>({
      method: 'POST',
      url: '/api/app/products/attribute',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: '/api/app/products',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/products/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteMultiple = (ids: string[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/products/multiple',
      params: { ids },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/products/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/products',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getListAll = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/products/all',
    },
    { apiName: this.apiName,...config });
  

  getListFilter = (input: ProductListFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/products/filter',
      params: { categoryId: input.categoryId, keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getProductAttributeAll = (productId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeValueDto[]>({
      method: 'GET',
      url: `/api/app/products/product-attribute-all/${productId}`,
    },
    { apiName: this.apiName,...config });
  

  getProductAttributes = (input: ProductAttributeListFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductAttributeValueDto>>({
      method: 'GET',
      url: '/api/app/products/product-attributes',
      params: { productId: input.productId, keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getSuggestNewCode = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/products/suggest-new-code',
    },
    { apiName: this.apiName,...config });
  

  getThumbnailImage = (fileName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/products/thumbnail-image',
      params: { fileName },
    },
    { apiName: this.apiName,...config });
  

  removeProductAttribute = (attributeId: string, id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/products/${id}/product-attribute/${attributeId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/products/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateAttribute = (id: string, input: AddUpdateProductAttributeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeValueDto>({
      method: 'PUT',
      url: `/api/app/products/${id}/attribute`,
      body: input,
    },
    { apiName: this.apiName,...config });
}