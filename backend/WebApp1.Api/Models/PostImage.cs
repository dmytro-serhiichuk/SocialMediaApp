namespace WebApp1.Api.Models;

public class PostImage
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string StorageKey { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }

    public int Order { get; set; }
}