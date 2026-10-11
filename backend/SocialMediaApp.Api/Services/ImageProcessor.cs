using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SocialMediaApp.Api.Utils;

namespace SocialMediaApp.Api.Services;

public class ImageProcessor : IImageProcessor
{
    public async Task<ProcessedImage> ProcessAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var input = file.OpenReadStream();

            var info = await Image.IdentifyAsync(input, cancellationToken);

            if (info.Width is > 10000 or < 200 || info.Height is > 10000 or < 200)
            {
                throw new ValidationException("Each side of the image must be between 200 and 10000 pixels.");
            }

            if ((double)info.Width / info.Height is > 4 or < 0.25)
            {
                throw new ValidationException("Image aspect ratio must be between 1:4 and 4:1.");
            }
            
            input.Position = 0;

            using var image = await Image.LoadAsync(input, cancellationToken);

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
        catch (ImageFormatException)
        {
            throw new ValidationException("Image format or content is invalid or not supported.");
        }
    }
}