using LadyRuth.API.Data;
using LadyRuth.API.DTOs.Products;
using LadyRuth.API.Entities;
using LadyRuth.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Services;

public class ProductService(AppDbContext db) : IProductService
{
    // ── Public ────────────────────────────────────────────────────────────────
    public async Task<PagedResult<ProductListDto>> GetPublicProductsAsync(
        int page, int pageSize, int? categoryId, string? search)
    {
        var query = db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => ToListDto(p))
            .ToListAsync();

        return new PagedResult<ProductListDto>
        {
            Items = items, TotalCount = total, Page = page, PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id, bool includeInactive = false)
    {
        var query = db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Variants)
            .Where(p => p.Id == id);

        if (!includeInactive) query = query.Where(p => p.IsActive);

        var product = await query.FirstOrDefaultAsync();
        return product is null ? null : ToDto(product);
    }

    // ── Admin ─────────────────────────────────────────────────────────────────
    public async Task<PagedResult<ProductListDto>> GetAdminProductsAsync(
        int page, int pageSize, int? categoryId, string? search)
    {
        var query = db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => ToListDto(p))
            .ToListAsync();

        return new PagedResult<ProductListDto>
        {
            Items = items, TotalCount = total, Page = page, PageSize = pageSize
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name        = dto.Name,
            Description = dto.Description,
            Price       = dto.Price,
            CategoryId  = dto.CategoryId,
            CreatedAt   = DateTime.UtcNow,
            Variants    = dto.Variants.Select(v => new ProductVariant
            {
                Colour        = v.Colour,
                Size          = v.Size,
                StockQuantity = v.StockQuantity,
                SKU           = v.SKU
            }).ToList()
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        return (await GetProductByIdAsync(product.Id, includeInactive: true))!;
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await db.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return null;

        product.Name        = dto.Name;
        product.Description = dto.Description;
        product.Price       = dto.Price;
        product.CategoryId  = dto.CategoryId;
        product.IsActive    = dto.IsActive;
        product.UpdatedAt   = DateTime.UtcNow;

        // Replace variants
        db.ProductVariants.RemoveRange(product.Variants);
        product.Variants = dto.Variants.Select(v => new ProductVariant
        {
            ProductId     = id,
            Colour        = v.Colour,
            Size          = v.Size,
            StockQuantity = v.StockQuantity,
            SKU           = v.SKU
        }).ToList();

        await db.SaveChangesAsync();
        return await GetProductByIdAsync(id, includeInactive: true);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return false;
        product.IsActive  = false;
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ProductImageDto> AddImageAsync(int productId, byte[] data, string contentType, bool isPrimary)
    {
        if (isPrimary)
        {
            var existing = await db.ProductImages
                .Where(i => i.ProductId == productId && i.IsPrimary)
                .ToListAsync();
            existing.ForEach(i => i.IsPrimary = false);
        }

        var maxOrder = await db.ProductImages
            .Where(i => i.ProductId == productId)
            .Select(i => (int?)i.SortOrder)
            .MaxAsync() ?? 0;

        var image = new ProductImage
        {
            ProductId   = productId,
            Data        = data,
            ContentType = contentType,
            IsPrimary   = isPrimary,
            SortOrder   = maxOrder + 1
        };

        db.ProductImages.Add(image);
        await db.SaveChangesAsync();

        return new ProductImageDto
        {
            Id        = image.Id,
            Url       = $"/api/products/images/{image.Id}",
            IsPrimary = image.IsPrimary,
            SortOrder = image.SortOrder
        };
    }

    public async Task<ProductImage?> GetImageDataAsync(int imageId)
        => await db.ProductImages.FindAsync(imageId);

    public async Task<bool> DeleteImageAsync(int productId, int imageId)
    {
        var image = await db.ProductImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId);
        if (image is null) return false;
        db.ProductImages.Remove(image);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Mapping helpers ───────────────────────────────────────────────────────
    private static ProductListDto ToListDto(Product p) => new()
    {
        Id              = p.Id,
        Name            = p.Name,
        Price           = p.Price,
        CategoryName    = p.Category.Name,
        PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary) is { } pri
                              ? $"/api/products/images/{pri.Id}"
                              : p.Images.OrderBy(i => i.SortOrder).FirstOrDefault() is { } first
                              ? $"/api/products/images/{first.Id}"
                              : null,
        IsActive        = p.IsActive,
        TotalStock      = p.Variants.Sum(v => v.StockQuantity)
    };

    private static ProductDto ToDto(Product p) => new()
    {
        Id           = p.Id,
        Name         = p.Name,
        Description  = p.Description,
        Price        = p.Price,
        CategoryId   = p.CategoryId,
        CategoryName = p.Category.Name,
        IsActive     = p.IsActive,
        CreatedAt    = p.CreatedAt,
        Images       = p.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto
        {
            Id        = i.Id,
            Url       = $"/api/products/images/{i.Id}",
            IsPrimary = i.IsPrimary,
            SortOrder = i.SortOrder
        }).ToList(),
        Variants = p.Variants.Select(v => new ProductVariantDto
        {
            Id            = v.Id,
            Colour        = v.Colour,
            Size          = v.Size,
            StockQuantity = v.StockQuantity,
            SKU           = v.SKU
        }).ToList()
    };
}
