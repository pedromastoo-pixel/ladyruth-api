namespace LadyRuth.API.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public byte[] Data { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
