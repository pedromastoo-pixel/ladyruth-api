namespace LadyRuth.API.DTOs.Testimonials;

public class TestimonialDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public int Rating { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpsertTestimonialDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public int Rating { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
