using LadyRuth.API.DTOs.Products;
using LadyRuth.API.Entities;

namespace LadyRuth.API.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListDto>> GetPublicProductsAsync(int page, int pageSize, int? categoryId, string? search);
    Task<ProductDto?> GetProductByIdAsync(int id, bool includeInactive = false);
    Task<PagedResult<ProductListDto>> GetAdminProductsAsync(int page, int pageSize, int? categoryId, string? search);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
    Task<ProductImageDto> AddImageAsync(int productId, byte[] data, string contentType, bool isPrimary);
    Task<ProductImage?> GetImageDataAsync(int imageId);
    Task<bool> DeleteImageAsync(int productId, int imageId);
}
