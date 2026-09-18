using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/")]
public class CommentsController(
    CommentsService commentsService
) : ControllerBase
{
    [Authorize]
    [HttpPost("posts/{postId}/comment")]
    public async Task<IActionResult> CommentPost([FromBody] CommentPostRequest request, [FromRoute] int postId)
    {
        var userId = User.GetUserId();

        var res = await commentsService.CommentPost(request, userId, postId);

        if (res is null) return BadRequest();

        return StatusCode(StatusCodes.Status201Created, res);
    }

    [HttpGet("posts/{postId}/[controller]")]
    public async Task<IActionResult> GetComments([FromQuery] PaginationRequest pagination, [FromRoute] int postId)
    {
        return Ok(await commentsService.GetPostComments(pagination, postId));
    }
}