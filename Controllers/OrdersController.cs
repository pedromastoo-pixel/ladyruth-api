using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        try
        {
            var order = await orderService.PlaceOrderAsync(dto);
            return CreatedAtAction(nameof(GetByOrderNumber),
                new { orderNumber = order.OrderNumber }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber)
    {
        var order = await orderService.GetByOrderNumberAsync(orderNumber);
        return order is null ? NotFound() : Ok(order);
    }
}
