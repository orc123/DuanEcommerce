using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DuanEcommerce.Admin.ProductCategories;

public class ProductCategoryInListDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public int SortOrder { get; set; }
    public string CoverPicture { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
}
