using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Extensions;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class SettingsController(
    SettingsService settingsService, 
    SessionService sessionService,
    AccountSecurityService accountSecurityService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var userId = User.GetUserId();
        var sessionId = User.GetSessionId();

        var res = await settingsService.GetData(userId, sessionId);

        if (res is null) return NotFound();

        return Ok(res);
    }

    [Authorize]
    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] SettingsProfileRequest profileRequest)
    {
        var userId = User.GetUserId();

        await settingsService.UpdateProfile(userId, profileRequest);

        return NoContent();
    }

    [Authorize]
    [HttpPost("email-change")]
    public async Task<IActionResult> RequestEmailChange([FromBody] EmailChangeRequest emailChangeRequest)
    {
        var userId = User.GetUserId();

        await settingsService.RequestEmailChange(userId, emailChangeRequest);

        return NoContent();
    }

    [HttpPost("email-change/confirm")]
    public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmationRequest confirmation)
    {
        await settingsService.ConfirmEmailChange(confirmation);

        return NoContent();
    }

    [Authorize]
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest passwordChangeRequest)
    {
        var userId = User.GetUserId();
        var sessionId = User.GetSessionId();

        await settingsService.ChangePassword(userId, passwordChangeRequest);
        
        await sessionService.TerminateAllSessionsButCurrent(userId, sessionId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("sessions/{sessionId:int}/terminate")]
    public async Task<IActionResult> TerminateSession([FromRoute] int sessionId)
    {
        var userId = User.GetUserId();

        var res = await sessionService.TerminateSessionById(userId, sessionId);

        if (!res) return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpPost("sessions/terminate")]
    public async Task<IActionResult> TerminateAllOtherSession()
    {
        var userId = User.GetUserId();
        var sessionId = User.GetSessionId();

        await sessionService.TerminateAllSessionsButCurrent(userId, sessionId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("password-create/request")]
    public async Task<IActionResult> RequestPasswordCreation([FromBody] RequestPasswordResetRequest request)
    {
        await accountSecurityService.RequestPasswordReset(request);
        
        return NoContent();
    }

    [Authorize]
    [HttpPost("password-create/complete")]
    public async Task<IActionResult> CompletePasswordCreation([FromBody] ResetPasswordRequest request)
    {
        await accountSecurityService.ConfirmPasswordReset(request);

        return NoContent();
    }
}