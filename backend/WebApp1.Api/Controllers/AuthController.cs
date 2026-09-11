using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Models;
using WebApp1.Api.Services;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AuthService authService, 
    SessionService sessionService, 
    JwtTokenService jwtService, 
    IOptions<JwtOptions> accessTokenOptions, 
    IOptions<RefreshTokenOptions> refreshTokenOptions,
    IOptions<FrontendOptions> frontendOptions,
    AccountSecurityService accountSecurityService) : ControllerBase
{
    private readonly JwtOptions accessTokenOptions = accessTokenOptions.Value;
    private readonly RefreshTokenOptions refreshTokenOptions = refreshTokenOptions.Value;
    private readonly FrontendOptions frontendOptions = frontendOptions.Value;

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.GetUserId();

        var user = await authService.GetById(userId);
        
        if (user is null) return NotFound();
        
        return Ok(UserResponse.CreateFromUser(user));
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        await authService.HandleRegisterRequest(registerRequest);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("register/confirm")]
    public async Task<IActionResult> ConfirmRegister([FromBody] ConfirmationRequest confirmationRequest)
    {
        var user = await authService.ConfirmRegister(confirmationRequest);

        return await SendRegisterRequest(user);
    }

    [HttpPost("register/complete")]
    public async Task<IActionResult> CompleteRegister([FromBody] RegisterCompletionRequest completionRequest)
    {
        var user = await authService.CompleteRegister(completionRequest);

        return await SendRegisterRequest(user);
    }

    private async Task<IActionResult> SendRegisterRequest(User user)
    {
        var sessionCreationResult = await sessionService.CreateSession(
            user, 
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString());

        var accessToken = jwtService.CreateAccessToken(user.Id, sessionCreationResult.Session.Id);

        SetTokenCookies(accessToken, sessionCreationResult.Token);

        return StatusCode(StatusCodes.Status201Created, UserResponse.CreateFromUser(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = await authService.Login(loginRequest);

        if (user is null) return Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Invalid credentials."
        });

        var sessionCreationResult = await sessionService.CreateSession(
            user, 
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString());

        var accessToken = jwtService.CreateAccessToken(user.Id, sessionCreationResult.Session.Id);

        SetTokenCookies(accessToken, sessionCreationResult.Token);

        return Ok(UserResponse.CreateFromUser(user));
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        Response.Cookies.Append(
            accessTokenOptions.AccessTokenCookieName,
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(accessTokenOptions.AccessTokenLifetimeMinutes)
            });
        
        Response.Cookies.Append(
            refreshTokenOptions.TokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(refreshTokenOptions.TokenLifetimeDays)
            });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[refreshTokenOptions.TokenCookieName];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await sessionService.DeleteByToken(refreshToken);
        }

        Response.Cookies.Delete(accessTokenOptions.AccessTokenCookieName);
        Response.Cookies.Delete(refreshTokenOptions.TokenCookieName);
        
        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[refreshTokenOptions.TokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var rotationResult = await sessionService.RotateRefreshToken(
            refreshToken,
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString());

        if (rotationResult is null)
        {
            return Unauthorized();
        }

        var accessToken = jwtService.CreateAccessToken(rotationResult.Session.UserId, rotationResult.Session.Id);

        SetTokenCookies(accessToken, rotationResult.Token);

        return NoContent();
    }

    [HttpPost("password-reset/request")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
    {
        await accountSecurityService.RequestPasswordReset(request);
        
        return NoContent();
    }

    [HttpPost("password-reset/confirm")]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordRequest request)
    {
        var user = await accountSecurityService.ConfirmPasswordReset(request);

        await sessionService.TerminateAllSessions(user.Id);
        
        return NoContent();
    }

    [HttpPost("acccount-recovery/secure")]
    public async Task<IActionResult> SecureAccount([FromBody] ConfirmationRequest confirmation)
    {
        var user = await accountSecurityService.StartRecoveryProcess(confirmation);

        await sessionService.TerminateAllSessions(user.Id);
        
        return NoContent();
    }

    [HttpPost("acccount-recovery/complete")]
    public async Task<IActionResult> CompleteAccountRecovery([FromBody] ResetPasswordRequest resetPassword)
    {
        await accountSecurityService.CompleteAccountRecovery(resetPassword);
                
        return NoContent();
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/auth/google-login-callback",
                Items = { ["errorRedirectPath"] = "/login" }
            },
            "Google");
    }

    [Authorize]
    [HttpGet("google/connect")]
    public IActionResult GoogleConnect()
    {
        return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/auth/google-connect-callback",
                Items = { ["errorRedirectPath"] = "/settings" }
            },
            "Google");
    }
    
    [TypeFilter(typeof(ExternalLoginExceptionFilter), Arguments = new object[] { "/login" })]
    [HttpGet("google-login-callback")]
    public async Task<IActionResult> GoogleLoginCallback()
    {
        var authResult = await HttpContext.AuthenticateAsync("External");

        if (!authResult.Succeeded)
        {
            await HttpContext.SignOutAsync("External");
            return Redirect($"{frontendOptions.Url}/login?error=login_failed");
        }

        var providerUserId = authResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = authResult.Principal.FindFirstValue(ClaimTypes.Name);
        var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);

        if (providerUserId is null || userName is null || email is null)
        {
            await HttpContext.SignOutAsync("External");
            return Redirect($"{frontendOptions.Url}/login?error=login_failed");
        }

        var result = await authService.HandleExternalLogin(new ExternalLoginInfo(
            ExternalLoginProvider.Google, providerUserId, email, userName)
        );

        return result switch
        {
            ExternalLoginResult.Success s => await CompleteProviderLogin(s.User),
            ExternalLoginResult.EmailAlreadyTaken => Redirect($"{frontendOptions.Url}/login?error=email_taken&provider={ExternalLoginProvider.Google}"),
            _ => Redirect($"{frontendOptions.Url}/login?error=unknown")
        };
    }

    private async Task<IActionResult> CompleteProviderLogin(User user)
    {
        var sessionCreationResult = await sessionService.CreateSession(
            user, 
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString());

        var accessToken = jwtService.CreateAccessToken(user.Id, sessionCreationResult.Session.Id);

        SetTokenCookies(accessToken, sessionCreationResult.Token);

        await HttpContext.SignOutAsync("External");

        return Redirect($"{frontendOptions.Url}/profile");
    }

    [TypeFilter(typeof(ExternalLoginExceptionFilter), Arguments = new object[] { "/settings" })]
    [HttpGet("google-connect-callback")]
    public async Task<IActionResult> GoogleConnectCallback()
    {
        var externalAuth = await HttpContext.AuthenticateAsync("External");
        var appAuth = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        if (!externalAuth.Succeeded || !appAuth.Succeeded)
        {
            await HttpContext.SignOutAsync("External");
            return Redirect($"{frontendOptions.Url}/settings?error=connect_failed");
        }

        var userId = appAuth.Principal.GetUserId();

        var providerUserId = externalAuth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = externalAuth.Principal.FindFirstValue(ClaimTypes.Name);
        var email = externalAuth.Principal.FindFirstValue(ClaimTypes.Email);

        if (providerUserId is null || userName is null || email is null)
        {
            await HttpContext.SignOutAsync("External");
            return Redirect($"{frontendOptions.Url}/settings?error=connect_failed");
        }

        var result = await authService.ConnectExternalLogin(userId, new ExternalLoginInfo(
            ExternalLoginProvider.Google, providerUserId, email, userName)
        );

        await HttpContext.SignOutAsync("External");

        return result switch
        {
            ConnectExternalLoginResult.Success => Redirect($"{frontendOptions.Url}/settings"),
            ConnectExternalLoginResult.AlreadyLinkedToThisAccount => Redirect($"{frontendOptions.Url}/settings"),
            ConnectExternalLoginResult.AlreadyLinkedToAnotherAccount => Redirect($"{frontendOptions.Url}/settings?error=already_linked&provider={ExternalLoginProvider.Google}"),
            _ => Redirect($"{frontendOptions.Url}/settings?error=unknown")
        };
    }
}