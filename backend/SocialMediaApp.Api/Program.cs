using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.Models;
using SocialMediaApp.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SocialMediaApp.Api.Configuration;
using SocialMediaApp.Api.Services.Email;
using Microsoft.AspNetCore.Authentication.OAuth;

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
    .Validate(options =>
        options.PasswordResetLifetimeMinutes > 0,
        "Password reset token lifetime must be greater than zero.")
    .Validate(options =>
        options.NewLoginTokenLifetimeMinutes > 0,
        "New login token lifetime must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName))
    .Validate(options =>
        !(string.IsNullOrWhiteSpace(options.Username) ^
         string.IsNullOrWhiteSpace(options.Password)),
        "SMTP Username and password must either both be specified or both be left unspecified"
    )
    .ValidateOnStart();


builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<ExpiredDataCleanupService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

builder.Services.AddScoped<IImageStorage, FileSystemImageStorage>();
builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<ImageService>();

builder.Services.AddScoped<IPasswordHasher<PendingRegistration>, PasswordHasher<PendingRegistration>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<AccountSecurityService>();
builder.Services.AddScoped<CommentService>();

var jwtKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT secret is missing.");

var frontendUrl = builder.Configuration["Frontend:Url"] 
        ?? throw new InvalidOperationException("Frontend Url is missing");

var authentication = builder.Services
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

var googleClientId = builder.Configuration["Auth:Google:ClientId"];
var googleClientSecret = builder.Configuration["Auth:Google:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authentication
        .AddCookie("External")
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;

            options.SignInScheme = "External";

            options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    var errorPath = "/login";
                    if (context.Properties?.Items.TryGetValue("errorRedirectPath", out var path) == true && path is not null)
                    {
                        errorPath = path;
                    }
                    context.HandleResponse();
                    context.Response.Redirect($"{frontendUrl}{errorPath}?error=oauth_failed");
                    return Task.CompletedTask;
                }
            };
        });
}

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{   
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
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseCors(FrontendOptions.SectionName);

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
