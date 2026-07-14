using System.Text;
using LadyRuth.API.Data;
using Resend;
using LadyRuth.API.Settings;
using LadyRuth.API.Middleware;
using LadyRuth.API.Services;
using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret  = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSection["Issuer"],
            ValidAudience            = jwtSection["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.Configure<PayFastSettings>(builder.Configuration.GetSection("PayFast"));
builder.Services.AddScoped<IPayFastService, PayFastService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["Resend:ApiKey"] ?? string.Empty;
});
builder.Services.AddTransient<IResend, ResendClient>();

// ── Infrastructure ────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "LadyRuth API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
    });
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer"), [] }
    });
});


var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Angular");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Auto-migrate on startup ───────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed — app will still start.");
    }
}

app.Run();
