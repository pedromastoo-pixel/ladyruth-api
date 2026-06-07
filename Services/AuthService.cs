using LadyRuth.API.Data;
using LadyRuth.API.DTOs.Auth;
using LadyRuth.API.Helpers;
using LadyRuth.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Services;

public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await db.AdminUsers
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var jwtHelper = new JwtHelper(config);
        var (token, expiresAt) = jwtHelper.Generate(user);

        return new LoginResponse
        {
            Token     = token,
            ExpiresAt = expiresAt,
            Email     = user.Email,
            Role      = user.Role
        };
    }
}
