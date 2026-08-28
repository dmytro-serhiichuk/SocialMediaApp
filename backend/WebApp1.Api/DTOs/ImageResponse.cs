namespace WebApp1.Api.DTOs;

public class ImageResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
}