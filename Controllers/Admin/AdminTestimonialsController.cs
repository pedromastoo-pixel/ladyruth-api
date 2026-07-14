using LadyRuth.API.DTOs.Testimonials;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers.Admin;

[ApiController]
[Route("api/admin/testimonials")]
[Authorize]
public class AdminTestimonialsController(ITestimonialService testimonialService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await testimonialService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await testimonialService.GetByIdAsync(id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertTestimonialDto dto) =>
        Ok(await testimonialService.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertTestimonialDto dto)
    {
        var t = await testimonialService.UpdateAsync(id, dto);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await testimonialService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
