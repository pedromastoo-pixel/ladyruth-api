using LadyRuth.API.Data;
using LadyRuth.API.DTOs.Testimonials;
using LadyRuth.API.Entities;
using LadyRuth.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Services;

public class TestimonialService(AppDbContext db) : ITestimonialService
{
    public async Task<List<TestimonialDto>> GetActiveAsync() =>
        await db.Testimonials
            .AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => ToDto(t))
            .ToListAsync();

    public async Task<List<TestimonialDto>> GetAllAsync() =>
        await db.Testimonials
            .AsNoTracking()
            .OrderBy(t => t.SortOrder)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => ToDto(t))
            .ToListAsync();

    public async Task<TestimonialDto?> GetByIdAsync(int id)
    {
        var t = await db.Testimonials.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return t is null ? null : ToDto(t);
    }

    public async Task<TestimonialDto> CreateAsync(UpsertTestimonialDto dto)
    {
        var t = new Testimonial
        {
            CustomerName = dto.CustomerName,
            Quote        = dto.Quote,
            ProductName  = dto.ProductName,
            Rating       = dto.Rating,
            IsActive     = dto.IsActive,
            SortOrder    = dto.SortOrder,
            CreatedAt    = DateTime.UtcNow
        };
        db.Testimonials.Add(t);
        await db.SaveChangesAsync();
        return ToDto(t);
    }

    public async Task<TestimonialDto?> UpdateAsync(int id, UpsertTestimonialDto dto)
    {
        var t = await db.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
        if (t is null) return null;

        t.CustomerName = dto.CustomerName;
        t.Quote        = dto.Quote;
        t.ProductName  = dto.ProductName;
        t.Rating       = dto.Rating;
        t.IsActive     = dto.IsActive;
        t.SortOrder    = dto.SortOrder;

        await db.SaveChangesAsync();
        return ToDto(t);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var t = await db.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
        if (t is null) return false;
        db.Testimonials.Remove(t);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<(byte[] Data, string ContentType)?> GetImageAsync(int id)
    {
        var t = await db.Testimonials.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (t?.ImageData is null || t.ImageContentType is null) return null;
        return (t.ImageData, t.ImageContentType);
    }

    public async Task<bool> SetImageAsync(int id, byte[] data, string contentType)
    {
        var t = await db.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
        if (t is null) return false;
        t.ImageData        = data;
        t.ImageContentType = contentType;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveImageAsync(int id)
    {
        var t = await db.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
        if (t is null) return false;
        t.ImageData        = null;
        t.ImageContentType = null;
        await db.SaveChangesAsync();
        return true;
    }

    private static TestimonialDto ToDto(Testimonial t) => new()
    {
        Id           = t.Id,
        CustomerName = t.CustomerName,
        Quote        = t.Quote,
        ProductName  = t.ProductName,
        Rating       = t.Rating,
        IsActive     = t.IsActive,
        SortOrder    = t.SortOrder,
        HasImage     = t.ImageData is not null,
        CreatedAt    = t.CreatedAt
    };
}
