using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DuanEcommerce.Public.Orders;

public interface IOrdersAppService : ICrudAppService
    <OrderDto, Guid, PagedAndSortedResultRequestDto, CreateOrderDto, CreateOrderDto>
{
}
