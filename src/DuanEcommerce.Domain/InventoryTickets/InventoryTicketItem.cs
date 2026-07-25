using System;
using System.Collections.Generic;
using System.Text;

namespace DuanEcommerce.InventoryTickets;

public class InventoryTicketItem
{
    public Guid TicketId { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; }
    public int Quantity { get; set; }
    public string BatchNumber { get; set; }
    public DateTime? ExpiredDate { get; set; }
}
