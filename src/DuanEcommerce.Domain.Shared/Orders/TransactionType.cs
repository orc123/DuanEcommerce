using System;
using System.Collections.Generic;
using System.Text;

namespace DuanEcommerce.Orders;

public enum TransactionType
{
    ConfirmOrder,
    StartProcessing,
    FinishOrder,
    CancelOrder
}
