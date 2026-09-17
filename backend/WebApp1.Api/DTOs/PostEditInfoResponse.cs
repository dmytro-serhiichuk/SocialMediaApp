namespace WebApp1.Api.DTOs;

public class PostEditInfoResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = null!;
    public List<ImageResponse> Images { get; set; } = null!;
}