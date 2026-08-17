namespace WebApp1.Api.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<PostTag> Posts { get; set; } = [];
}