using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserService userService, JwtTokenService jwtService, IOptions<JwtOptions> options) : ControllerBase
{
    private readonly JwtOptions jwtOptions = options.Value;

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.GetUserId();

        var user = await userService.GetById(userId);
        
        if (user is null) return NotFound();
        
        return Ok(UserResponse.CreateFromUser(user));
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var user = await userService.Create(registerRequest);

        if (user is null) return Conflict(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "User already exists.",
            Detail = "A user with the provided user tag or email already exists."
        });

        var accessToken = jwtService.CreateAccessToken(user);

        SetAccessTokenCookie(accessToken);

        return StatusCode(StatusCodes.Status201Created, UserResponse.CreateFromUser(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = await userService.Login(loginRequest);

        if (user is null) return Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Invalid credentials."
        });

        var accessToken = jwtService.CreateAccessToken(user);

        SetAccessTokenCookie(accessToken);

        return Ok(UserResponse.CreateFromUser(user));
    }

    private void SetAccessTokenCookie(string accessToken)
    {
        Response.Cookies.Append(
            jwtOptions.AccessTokenCookieName,
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(jwtOptions.AccessTokenLifetimeMinutes)
            });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // TODO: handle refresh tokens
        var userId = User.GetUserId();
        var user = await userService.GetById(userId);

        Response.Cookies.Delete(jwtOptions.AccessTokenCookieName);
        
        return NoContent();
    }
}