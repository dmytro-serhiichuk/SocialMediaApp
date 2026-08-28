namespace WebApp1.Api.Models;

public class PostImage
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public Guid ImageId { get; set; }
    public Image Image { get; set; } = null!;

    public int Order { get; set; }
}