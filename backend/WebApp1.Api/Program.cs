using WebApp1.Api.Data;
using WebApp1.Api.Models;
using WebApp1.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp1.Api.Configuration;
using WebApp1.Api.Services.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddOptions<FrontendOptions>()
    .Bind(builder.Configuration.GetSection(FrontendOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.Url),
        "Frontend Url is missing.")
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.SecretKey),
        "JWT secret key is required.")
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.Issuer),
        "JWT issuer is required.")
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.Audience),
        "JWT audience is required.")
    .Validate(options =>
        options.AccessTokenLifetimeMinutes > 0,
        "Access token lifetime must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddOptions<RefreshTokenOptions>()
    .Bind(builder.Configuration.GetSection(RefreshTokenOptions.SectionName))
    .Validate(options =>
        options.TokenLifetimeDays > 0,
        "Refresh token lifetime must be greater than zero.")
    .Validate(options =>
        options.GraceTimeSeconds > 0,
        "Refresh token grace time must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddOptions<ConfiramtionTokenOptions>()
    .Bind(builder.Configuration.GetSection(ConfiramtionTokenOptions.SectionName))
    .Validate(options =>
        options.EmailChangeLifetimeMinutes > 0,
        "Email change token lifetime must be greater than zero.")
    .Validate(options =>
        options.EmailConfirmationLifetimeHours > 0,
        "Email confirmation token lifetime must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName));


builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddScoped<IImageStorage, FileSystemImageStorage>();
builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<ImageService>();

builder.Services.AddScoped<IPasswordHasher<PendingRegistration>, PasswordHasher<PendingRegistration>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<SettingsService>();

var jwtKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT secret is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{   
    var frontendUrl = builder.Configuration["Frontend:Url"] 
        ?? throw new InvalidOperationException("Frontend Url is missing");

    options.AddPolicy(FrontendOptions.SectionName, policy =>
    {
        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(FrontendOptions.SectionName);

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
