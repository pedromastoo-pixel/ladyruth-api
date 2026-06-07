using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
    {
        var result = await productService.GetPublicProductsAsync(page, pageSize, categoryId, search);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("images/{imageId:int}")]
    public async Task<IActionResult> GetImage(int imageId)
    {
        var image = await productService.GetImageDataAsync(imageId);
        if (image is null) return NotFound();
        return File(image.Data, image.ContentType);
    }
}
