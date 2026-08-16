using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.DTOs;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class UsersController(UserService userService) : ControllerBase
{
    [HttpGet("{tag}")]
    public async Task<IActionResult> GetUser([FromRoute] string tag)
    {
         var user = await userService.GetByUserTag(tag);
        
        if (user is null) return NotFound();
        
        return Ok(UserProfileResponse.CreateFromUser(user));
    }
}