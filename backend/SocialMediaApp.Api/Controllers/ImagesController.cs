using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class ImagesController(ImageService imageService) : ControllerBase
{
    [HttpGet("{imageId:guid}")]
    public async Task<IActionResult> GetImage(Guid imageId)
    {
        var imagePath = await imageService.GetImagePathAsync(imageId);

        if (imagePath is null) return NotFound();

        return PhysicalFile(imagePath, "image/webp");
    }
}