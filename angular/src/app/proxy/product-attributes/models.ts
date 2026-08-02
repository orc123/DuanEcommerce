import type { AttributeType } from '../duan-ecommerce/product-attributes/attribute-type.enum';
import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateProductAttributeDto {
  code?: string;
  dataType?: AttributeType;
  label?: string;
  visibility?: boolean;
  isActive?: boolean;
  isRequired?: boolean;
  isUnique?: boolean;
  note?: string | null;
}

export interface ProductAttributeDto extends EntityDto<string> {
  id?: string;
  code?: string;
  dataType?: AttributeType;
  label?: string;
  visibility?: boolean;
  isActive?: boolean;
  isRequired?: boolean;
  isUnique?: boolean;
  note?: string | null;
}
