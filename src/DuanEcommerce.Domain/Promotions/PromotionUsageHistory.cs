using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Promotions;

public class PromotionUsageHistory : Entity<Guid>
{
    public Guid PromotionId { get; set; }
    public Guid OrderId { get; set; }

}