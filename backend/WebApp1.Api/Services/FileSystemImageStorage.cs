
namespace WebApp1.Api.Services;

public class FileSystemImageStorage(IConfiguration configuration, IWebHostEnvironment environment) : IImageStorage
{
    private readonly string _rootPath = Path.Combine(environment.ContentRootPath, configuration["Storage:RootPath"])
            ?? throw new InvalidOperationException(
                "Storage root path is not configured.");

    public async Task SaveAsync(Stream stream, string storageKey, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_rootPath, storageKey);

        var directory = Path.GetDirectoryName(path)!;

        Directory.CreateDirectory(directory);

        await using var fileStream = File.Create(path);

        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_rootPath, storageKey);

        if (File.Exists(path)) File.Delete(path);

        return Task.CompletedTask;
    }

    public string GetPath(string storageKey)
    {
        return Path.Combine(_rootPath, storageKey);
    }
}