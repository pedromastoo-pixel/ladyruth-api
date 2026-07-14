using LadyRuth.API.DTOs.Testimonials;

namespace LadyRuth.API.Services.Interfaces;

public interface ITestimonialService
{
    Task<List<TestimonialDto>> GetActiveAsync();
    Task<List<TestimonialDto>> GetAllAsync();
    Task<TestimonialDto?> GetByIdAsync(int id);
    Task<TestimonialDto> CreateAsync(UpsertTestimonialDto dto);
    Task<TestimonialDto?> UpdateAsync(int id, UpsertTestimonialDto dto);
    Task<bool> DeleteAsync(int id);
    Task<(byte[] Data, string ContentType)?> GetImageAsync(int id);
    Task<bool> SetImageAsync(int id, byte[] data, string contentType);
    Task<bool> RemoveImageAsync(int id);
}
