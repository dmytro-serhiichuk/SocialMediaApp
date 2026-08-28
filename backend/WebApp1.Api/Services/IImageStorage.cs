namespace WebApp1.Api.Services;

public interface IImageStorage
{
    Task SaveAsync(Stream stream, string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);

    string GetPath(string storageKey);
}