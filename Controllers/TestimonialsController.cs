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
}
