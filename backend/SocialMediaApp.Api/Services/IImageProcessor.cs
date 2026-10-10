using SocialMediaApp.Api.Utils;

namespace SocialMediaApp.Api.Services;

public interface IImageProcessor
{
    Task<ProcessedImage> ProcessAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}