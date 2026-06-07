using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await categoryService.GetAllAsync());
}
