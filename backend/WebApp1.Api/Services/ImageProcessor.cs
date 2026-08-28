using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class ImageProcessor : IImageProcessor
{
    public async Task<ProcessedImage> ProcessAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var input = file.OpenReadStream();

        using var image = await Image.LoadAsync(input, cancellationToken);

        var ratio = (double)image.Width / image.Height;

        if (ratio is > 4 or < 0.25)
        {
            throw new ValidationException("Image aspect ratio must be between 1:4 and 4:1.");
        }

        image.Mutate(x =>
        {
            x.AutoOrient();
            x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(2048, 2048)
            });
        });

        var output = new MemoryStream();

        await image.SaveAsWebpAsync(output, new WebpEncoder
        {
            Quality = 80
        }, cancellationToken);

        output.Position = 0;

        return new ProcessedImage
        {
            Content = output,
            Width = image.Width,
            Height = image.Height
        };
    }
}