namespace WebApp1.Api.DTOs;

public class PostPaginationResponse
{
    public List<PostResponse> Posts { get; set; } = null!;
    public bool HasMore { get; set; }
    public string NextCursor { get; set; } = string.Empty;
}