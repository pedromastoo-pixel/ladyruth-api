using System.ComponentModel.DataAnnotations;

namespace LadyRuth.API.DTOs.Orders;

public class PlaceOrderDto
{
    [Required, EmailAddress, MaxLength(200)]
    public string GuestEmail { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string GuestFirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string GuestLastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string GuestPhone { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = [];
}

public class OrderItemRequest
{
    [Required]
    public int VariantId { get; set; }

    [Required, Range(1, 100)]
    public int Quantity { get; set; }
}
