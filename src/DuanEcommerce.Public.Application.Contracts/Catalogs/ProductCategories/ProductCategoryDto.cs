using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DuanEcommerce.Public.ProductCategories;

public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public int SortOrder { get; set; }
    public string? CoverPicture { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public string? SeoMetaDescription { get; set; }

    public List<ProductCategoryDto> Children { get; set; } = new List<ProductCategoryDto>();
}
