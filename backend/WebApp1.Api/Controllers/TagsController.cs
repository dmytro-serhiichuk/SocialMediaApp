using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.DTOs;
using WebApp1.Api.Services;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class TagsController(TagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> FindTagsWithQuery([FromQuery] TagSearchRequest searchRequest)
    {
        return Ok(await tagService.FindTagsByName(searchRequest));
    }
}