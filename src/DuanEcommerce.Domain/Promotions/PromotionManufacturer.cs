using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Promotions;

public class PromotionManufacturer : Entity<Guid>
{
    public Guid ManufactureId { get; set; }
    public Guid PromotionId { get; set; }

}