namespace LadyRuth.API.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Colour { get; set; } = string.Empty;

    /// <summary>S | M | L | XL | 2XL</summary>
    public string Size { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
    public string? SKU { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
