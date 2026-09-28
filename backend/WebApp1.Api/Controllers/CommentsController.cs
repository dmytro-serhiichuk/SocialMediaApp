using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/")]
public class CommentsController(
    CommentService commentService
) : ControllerBase
{
    [Authorize]
    [HttpPost("posts/{postId}/comment")]
    public async Task<IActionResult> CommentPost([FromBody] CommentRequest request, [FromRoute] int postId)
    {
        var userId = User.GetUserId();

        var res = await commentService.CommentPost(request, userId, postId);

        if (res is null) return BadRequest();

        return StatusCode(StatusCodes.Status201Created, res);
    }

    [Authorize]
    [HttpPost("posts/{postId}/[controller]/{commentId}/reply")]
    public async Task<IActionResult> ReplyComment([FromBody] CommentRequest request, [FromRoute] int postId, [FromRoute] int commentId)
    {
        var userId = User.GetUserId();

        var res = await commentService.ReplyComment(request, userId, postId, commentId);

        if (res is null) return BadRequest();

        return StatusCode(StatusCodes.Status201Created, res);
    }

    [HttpGet("posts/{postId}/[controller]")]
    public async Task<IActionResult> GetComments([FromQuery] PaginationRequest pagination, [FromRoute] int postId)
    {
        return Ok(await commentService.GetPostComments(pagination, postId));
    }

    [HttpGet("posts/{postId}/[controller]/{commentId}/replies")]
    public async Task<IActionResult> GetReplies([FromQuery] PaginationRequest pagination, [FromRoute] int postId, [FromRoute] int commentId)
    {
        return Ok(await commentService.GetCommentReplies(pagination, postId, commentId));
    }

    [Authorize]
    [HttpPost("[controller]/{id}/likes")]
    public async Task<IActionResult> LikeComment([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await commentService.LikeComment(id, userId);
        return Created();
    }

    [Authorize]
    [HttpDelete("[controller]/{id}/likes")]
    public async Task<IActionResult> UnlikeComment([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await commentService.UnlikeComment(id, userId);
        return NoContent();
    }
}