import type { BaseListFilterDto } from '../../models';
import type { EntityDto } from '@abp/ng.core';
import type { AttributeType } from '../../duan-ecommerce/product-attributes/attribute-type.enum';

export interface AddUpdateProductAttributeDto {
  productId?: string;
  attributeId?: string;
  dateTimeValue?: string | null;
  decimalValue?: number | null;
  intValue?: number | null;
  varcharValue?: string | null;
  textValue?: string | null;
}

export interface ProductAttributeListFilterDto extends BaseListFilterDto {
  productId?: string;
}

export interface ProductAttributeValueDto extends EntityDto<string> {
  productId?: string;
  attributeId?: string;
  code?: string;
  dataType?: AttributeType;
  label?: string;
  dateTimeValue?: string | null;
  decimalValue?: number | null;
  intValue?: number | null;
  textValue?: string | null;
  varcharValue?: string | null;
  dateTimeId?: string | null;
  decimalId?: string | null;
  intId?: string | null;
  textId?: string | null;
  varcharId?: string | null;
}
