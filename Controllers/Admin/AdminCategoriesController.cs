using LadyRuth.API.DTOs.Admin;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers.Admin;

[ApiController]
[Route("api/admin/categories")]
[Authorize]
public class AdminCategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await categoryService.GetAllAsync(includeInactive: true));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        => Ok(await categoryService.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
