namespace WebApp1.Api.Utils;

public class ProcessedImage
{
    public Stream Content { get; init; } = null!;
    public int Width { get; init; }
    public int Height { get; init; }
}