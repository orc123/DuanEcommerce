using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace DuanEcommerce.Promotions;

public class PromotionProduct : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public Guid PromotionId { get; set; }

}