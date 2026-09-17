namespace WebApp1.Api.DTOs;

public class ImageResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }

    public static string GetApiURL(Guid id)
    {
        return $"/api/images/{id}";
    }
}