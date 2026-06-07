using LadyRuth.API.DTOs.Products;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize]
public class AdminProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
        => Ok(await productService.GetAdminProductsAsync(page, pageSize, categoryId, search));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetProductByIdAsync(id, includeInactive: true);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = await productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await productService.UpdateProductAsync(id, dto);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productService.DeleteProductAsync(id);
        return result ? NoContent() : NotFound();
    }

    // ── Image management ──────────────────────────────────────────────────────
    [HttpPost("{id:int}/images")]
    public async Task<IActionResult> AddImage(int id, IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var isPrimary = !await HasAnyImageAsync(id);
        var image = await productService.AddImageAsync(id, ms.ToArray(), file.ContentType, isPrimary);
        return Ok(image);
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var result = await productService.DeleteImageAsync(id, imageId);
        return result ? NoContent() : NotFound();
    }

    private async Task<bool> HasAnyImageAsync(int productId)
    {
        var product = await productService.GetProductByIdAsync(productId, includeInactive: true);
        return product?.Images.Count > 0;
    }
}
