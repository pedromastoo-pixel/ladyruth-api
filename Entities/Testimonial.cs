namespace LadyRuth.API.Entities;

public class Testimonial
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public int Rating { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
