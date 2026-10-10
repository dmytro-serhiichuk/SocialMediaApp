using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.Extensions;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/")]
public class PostsController(PostService postService) : ControllerBase
{
    [Authorize]
    [HttpPost("[controller]/create")]
    public async Task<IActionResult> Create([FromForm] PostUpdateRequest creationRequest)
    {
        var userId = User.GetUserId();

        await postService.Create(creationRequest, userId);

        return StatusCode(StatusCodes.Status201Created);
    }

    [Authorize]
    [HttpPut("[controller]/{id}/update")]
    public async Task<IActionResult> Update([FromForm] PostUpdateRequest updateRequest, [FromRoute] int id)
    {
        var userId = User.GetUserId();

        if (!await postService.UpdatePost(updateRequest, userId, id))
        {
            return BadRequest();
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("[controller]/{id}")]
    public async Task<IActionResult> GetPost([FromRoute] int id)
    {
        var post = await postService.GetPostById(id);
        if (post is null) return NotFound();
        return Ok(post);
    }

    [HttpGet("[controller]")]
    public async Task<IActionResult> GetPosts([FromQuery] PaginationRequest postPagination)
    {
        return Ok(await postService.GetPosts(postPagination));
    }

    [HttpGet("users/{userId}/[controller]")]
    public async Task<IActionResult> GetPostsByUser([FromQuery] PaginationRequest postPagination, [FromRoute] int userId)
    {
        return Ok(await postService.GetPostsByUser(postPagination, userId));
    }

    [HttpGet("tags/{tagId}/[controller]")]
    public async Task<IActionResult> GetPostsByTag([FromQuery] PaginationRequest postPagination, [FromRoute] int tagId)
    {
        return Ok(await postService.GetPostsByTag(postPagination, tagId));
    }

    [Authorize]
    [HttpGet("[controller]/saved")]
    public async Task<IActionResult> GetSavedPosts([FromQuery] PaginationRequest postPagination)
    {
        var userId = User.GetUserId();
        return Ok(await postService.GetSavedPosts(postPagination, userId));
    }

    [Authorize]
    [HttpGet("[controller]/following")]
    public async Task<IActionResult> GetFollowingUsersPosts([FromQuery] PaginationRequest postPagination)
    {
        var userId = User.GetUserId();
        return Ok(await postService.GetFollowingPosts(postPagination, userId));
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

    [Authorize]
    [HttpGet("[controller]/{id}/edit")]
    public async Task<IActionResult> GetPostEditInfo([FromRoute] int id)
    {
        var userId = User.GetUserId();
        var post = await postService.GetPostEditInfo(id, userId);
        if (post is null) return NotFound();
        return Ok(post);
    }
}