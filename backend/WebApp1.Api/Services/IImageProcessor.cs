using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public interface IImageProcessor
{
    Task<ProcessedImage> ProcessAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}