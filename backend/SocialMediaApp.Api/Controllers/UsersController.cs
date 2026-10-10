using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Extensions;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class UsersController(UserService userService) : ControllerBase
{
    [HttpGet("{tag}")]
    public async Task<IActionResult> GetUser([FromRoute] string tag)
    {
        var user = await userService.GetUserProfile(tag);
        
        if (user is null) return NotFound();
        
        return Ok(user);
    }

    [Authorize]
    [HttpPost("{id}/follow")]
    public async Task<IActionResult> Follow([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await userService.FollowUser(id, userId);
        return Created();
    }

    [Authorize]
    [HttpDelete("{id}/follow")]
    public async Task<IActionResult> Unfollow([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await userService.UnfollowUser(id, userId);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindUsersWithQuery([FromQuery] UserSearchRequest searchRequest)
    {
        return Ok(await userService.FindUsersByTag(searchRequest));
    }

    [HttpGet("{tag}/followers")]
    public async Task<IActionResult> GetUserFollowers([FromRoute] string tag, [FromQuery] PaginationRequest paginationRequest)
    {
        return Ok(await userService.GetUserFollowers(paginationRequest, tag));
    }

    [HttpGet("{tag}/following")]
    public async Task<IActionResult> GetUserFollowing([FromRoute] string tag, [FromQuery] PaginationRequest paginationRequest)
    {
        return Ok(await userService.GetUserFollowing(paginationRequest, tag));
    }
}