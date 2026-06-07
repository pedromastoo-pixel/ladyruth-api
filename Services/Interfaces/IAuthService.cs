using LadyRuth.API.DTOs.Auth;

namespace LadyRuth.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
