using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/testimonials")]
public class TestimonialsController(ITestimonialService testimonialService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActive() =>
        Ok(await testimonialService.GetActiveAsync());

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var result = await testimonialService.GetImageAsync(id);
        if (result is null) return NotFound();
        return File(result.Value.Data, result.Value.ContentType);
    }
}
