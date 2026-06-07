using LadyRuth.API.Data;
using LadyRuth.API.DTOs.Admin;
using LadyRuth.API.Entities;
using LadyRuth.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Services;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id           = c.Id,
                Name         = c.Name,
                Slug         = c.Slug,
                IsActive     = c.IsActive,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .ToListAsync();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var slug = dto.Slug ?? dto.Name.ToLower().Replace(" ", "-");
        var category = new Category { Name = dto.Name, Slug = slug };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return new CategoryDto { Id = category.Id, Name = category.Name, Slug = category.Slug, IsActive = true };
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return null;
        category.Name = dto.Name;
        category.Slug = dto.Slug ?? dto.Name.ToLower().Replace(" ", "-");
        await db.SaveChangesAsync();
        return new CategoryDto { Id = category.Id, Name = category.Name, Slug = category.Slug, IsActive = category.IsActive };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return false;
        category.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }
}
