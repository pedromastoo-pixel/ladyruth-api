using LadyRuth.API.DTOs.Orders;

namespace LadyRuth.API.Services.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(OrderDto order);
    Task SendStatusUpdateAsync(OrderDto order);
}
