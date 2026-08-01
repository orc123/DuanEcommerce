using DuanEcommerce.ProductAttributes;
using System;

namespace DuanEcommerce.Admin.ProductAttributes;

public class CreateUpdateProductAttributeDto
{
    public string Code { get; set; }
    public AttributeType DataType { get; set; }
    public string Label { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUnique { get; set; }
    public string? Note { get; set; }
}
