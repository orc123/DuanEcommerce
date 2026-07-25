using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace DuanEcommerce.Inventories;

public class Inventory : AuditedAggregateRoot<Guid>
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; }
    public int StockQuantity { get; set; }
}
