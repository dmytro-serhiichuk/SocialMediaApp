using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class TagsController(TagService tagService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> FindTagsWithQuery([FromQuery] TagSearchRequest searchRequest)
    {
        return Ok(await tagService.FindTagsByName(searchRequest));
    }

    [HttpGet("{tagName}")]
    public async Task<IActionResult> GetFullTag([FromRoute] string tagName)
    {
        var tag = await tagService.GetTagByName(tagName);
        if (tag == null) return NotFound();
        return Ok(tag);
    }
}