using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]/")]
public class SettingsController(SettingsService settingsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var userId = User.GetUserId();

        var res = await settingsService.GetData(userId);

        if (res is null) return NotFound();

        return Ok(res);
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] SettingsProfileRequest profileRequest)
    {
        var userId = User.GetUserId();

        await settingsService.UpdateProfile(userId, profileRequest);

        return NoContent();
    }
}