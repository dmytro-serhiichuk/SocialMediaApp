namespace WebApp1.Api.Models;

public class Image
{
    public Guid Id { get; set; }
    public string StorageKey { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
}