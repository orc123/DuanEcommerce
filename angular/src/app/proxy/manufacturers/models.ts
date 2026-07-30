import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateManufacturerDto {
  name?: string;
  code?: string;
  slug?: string;
  coverPicture?: string;
  visibility?: boolean;
  isActive?: boolean;
  country?: string;
}

export interface ManufacturerDto extends EntityDto<string> {
  name?: string;
  code?: string;
  slug?: string;
  coverPicture?: string;
  visibility?: boolean;
  isActive?: boolean;
  country?: string;
}
