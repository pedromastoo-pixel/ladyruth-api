using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.Entities.Enums;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers.Admin;

[ApiController]
[Route("api/admin/email-test")]
[Authorize]
public class AdminEmailTestController(IEmailService emailService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendTest([FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(to)) return BadRequest("Provide a 'to' email address.");

        var order = new OrderDto
        {
            Id             = 0,
            OrderNumber    = "LR-TEST-001",
            GuestEmail     = to,
            GuestFirstName = "Monti",
            GuestLastName  = "Maswanganye",
            GuestPhone     = "+27 000 000 0000",
            AddressLine1   = "12 Jacaranda Street",
            AddressLine2   = "Apt 4B",
            City           = "Sandton",
            Province       = "Gauteng",
            PostalCode     = "2196",
            SubTotal       = 1597.00m,
            ShippingFee    = 100.00m,
            Total          = 1697.00m,
            Status         = "Processing",
            CreatedAt      = DateTime.UtcNow,
            Items          =
            [
                new OrderItemDto { Id = 1, ProductName = "Floral Wrap Dress", Colour = "Blush Pink", Size = "M", Quantity = 1, UnitPrice = 699.00m, LineTotal = 699.00m },
                new OrderItemDto { Id = 2, ProductName = "Satin Midi Skirt",  Colour = "Champagne",  Size = "S", Quantity = 2, UnitPrice = 449.00m, LineTotal = 898.00m }
            ]
        };

        await emailService.SendOrderConfirmationAsync(order);
        return Ok(new { message = $"Test email sent to {to}" });
    }
}
