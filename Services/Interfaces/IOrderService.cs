using LadyRuth.API.DTOs.Admin;
using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.DTOs.Products;
using LadyRuth.API.Entities.Enums;

namespace LadyRuth.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(PlaceOrderDto dto);
    Task<OrderDto?> GetByOrderNumberAsync(string orderNumber);
    Task<PagedResult<OrderDto>> GetAdminOrdersAsync(int page, int pageSize, OrderStatus? status);
    Task<OrderDto?> GetAdminOrderByIdAsync(int id);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto);
}
