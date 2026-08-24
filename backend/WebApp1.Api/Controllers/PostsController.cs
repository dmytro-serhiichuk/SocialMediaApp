using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.Extensions;
using WebApp1.Api.DTOs;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/")]
public class PostsController(PostService postService) : ControllerBase
{
    [Authorize]
    [HttpPost("[controller]/create")]
    public async Task<IActionResult> Create([FromBody] PostCreationRequest creationRequest)
    {
        var userId = User.GetUserId();

        await postService.Create(creationRequest, userId);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("[controller]")]
    public async Task<IActionResult> GetPosts([FromQuery] PostPaginationRequest postPagination)
    {
        return Ok(await postService.GetPosts(postPagination));
    }

    [HttpGet("users/{userId}/[controller]")]
    public async Task<IActionResult> GetPostsByUser([FromQuery] PostPaginationRequest postPagination, [FromRoute] int userId)
    {
        return Ok(await postService.GetPostsByUser(postPagination, userId));
    }

    [HttpGet("tags/{tagId}/[controller]")]
    public async Task<IActionResult> GetPostsByTag([FromQuery] PostPaginationRequest postPagination, [FromRoute] int tagId)
    {
        return Ok(await postService.GetPostsByTag(postPagination, tagId));
    }

    [Authorize]
    [HttpGet("[controller]/saved")]
    public async Task<IActionResult> GetSavedPosts([FromQuery] PostPaginationRequest postPagination)
    {
        var userId = User.GetUserId();
        return Ok(await postService.GetSavedPosts(postPagination, userId));
    }


    [Authorize]
    [HttpPost("[controller]/{id}/likes")]
    public async Task<IActionResult> LikePost([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await postService.LikePost(id, userId);
        return Created();
    }

    [Authorize]
    [HttpDelete("[controller]/{id}/likes")]
    public async Task<IActionResult> UnlikePost([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await postService.UnlikePost(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("[controller]/{id}/save")]
    public async Task<IActionResult> SavePost([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await postService.SavePost(id, userId);
        return Created();
    }

    [Authorize]
    [HttpDelete("[controller]/{id}/save")]
    public async Task<IActionResult> UnsavePost([FromRoute] int id)
    {
        var userId = User.GetUserId();
        await postService.UnsavePost(id, userId);
        return NoContent();
    }
}