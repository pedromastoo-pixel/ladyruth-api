namespace LadyRuth.API.DTOs.Orders;

public class PlaceOrderResponseDto
{
    public OrderDto Order { get; set; } = null!;
    public string PayFastUrl { get; set; } = string.Empty;
    public Dictionary<string, string> PayFastFields { get; set; } = [];
}
