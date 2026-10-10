namespace SocialMediaApp.Api.DTOs;

public class PaginationResponse<T>
{
    public List<T> Items { get; set; } = null!;
    public bool HasMore { get; set; }
    public string NextCursor { get; set; } = string.Empty;
}