import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateProductCategoryDto {
  name?: string;
  code?: string;
  slug?: string;
  sortOrder?: number;
  coverPicture?: string;
  visibility?: boolean;
  isActive?: boolean;
  parentId?: string | null;
  seoMetaDescription?: string;
}

export interface ProductCategoryDto extends EntityDto<string> {
  id?: string;
  name?: string;
  code?: string;
  slug?: string;
  sortOrder?: number;
  coverPicture?: string;
  visibility?: boolean;
  isActive?: boolean;
  parentId?: string | null;
  seoMetaDescription?: string;
}
