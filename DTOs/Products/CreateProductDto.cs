using System.ComponentModel.DataAnnotations;

namespace LadyRuth.API.DTOs.Products;

public class CreateProductDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public List<CreateVariantDto> Variants { get; set; } = [];
}

public class CreateVariantDto
{
    [Required, MaxLength(50)]
    public string Colour { get; set; } = string.Empty;

    [Required]
    public string Size { get; set; } = string.Empty;   // S|M|L|XL|2XL

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public string? SKU { get; set; }
}
