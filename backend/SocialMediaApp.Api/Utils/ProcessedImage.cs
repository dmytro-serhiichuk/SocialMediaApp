namespace SocialMediaApp.Api.Utils;

public class ProcessedImage : IDisposable
{
    public Stream Content { get; init; } = null!;
    public int Width { get; init; }
    public int Height { get; init; }

    public void Dispose()
    {
        Content.Dispose();
    }
}