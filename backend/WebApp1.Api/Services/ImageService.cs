using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.Models;

namespace WebApp1.Api.Services;

public class ImageService(IImageStorage imageStorage, IImageProcessor imageProcessor, AppDbContext dbContext)
{
    public async Task<string?> GetImagePathAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await dbContext.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageId, cancellationToken);

        if (image is null) return null;

        var path = imageStorage.GetPath(image.StorageKey);

        if (!File.Exists(path)) return null;

        return path;
    }

    public async Task<Image> CreateAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        using var processed = await imageProcessor.ProcessAsync(file, cancellationToken);
        
        var imageId = Guid.NewGuid();

        var image = new Image
        {
            Id = imageId,
            StorageKey = $"images/{imageId}.webp",
            Width = processed.Width,
            Height = processed.Height
        };

        await imageStorage.SaveAsync(processed.Content, image.StorageKey, cancellationToken);

        dbContext.Images.Add(image);

        return image;
    }

    public async Task DeleteStorageFileAsync(Image image, CancellationToken cancellationToken = default)
    {
        await imageStorage.DeleteAsync(image.StorageKey, cancellationToken);
    }
}