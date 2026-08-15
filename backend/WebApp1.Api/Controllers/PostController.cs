using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.Extensions;
using WebApp1.Api.Models;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(PostService postService) : ControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] PostCreationRequest creationRequest)
    {
        var userId = User.GetUserId();

        await postService.Create(creationRequest, userId);

        return StatusCode(StatusCodes.Status201Created);
    }
}