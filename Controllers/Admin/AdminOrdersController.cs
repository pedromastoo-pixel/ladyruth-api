using LadyRuth.API.DTOs.Admin;
using LadyRuth.API.Entities.Enums;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize]
public class AdminOrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] OrderStatus? status = null)
        => Ok(await orderService.GetAdminOrdersAsync(page, pageSize, status));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await orderService.GetAdminOrderByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await orderService.UpdateOrderStatusAsync(id, dto);
        return order is null ? NotFound() : Ok(order);
    }
}
