using System.ComponentModel.DataAnnotations;

namespace LadyRuth.API.DTOs.Products;

public class UpdateProductDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; }

    public List<CreateVariantDto> Variants { get; set; } = [];
}
