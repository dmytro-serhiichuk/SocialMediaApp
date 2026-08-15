using WebApp1.Api.Data;
using WebApp1.Api.Models;

namespace WebApp1.Api.Services;

public class PostService(AppDbContext dbContext)
{
    public async Task Create(PostCreationRequest creationRequest, int userId)
    {
        dbContext.Posts.Add(new Post
        {
            UserId = userId,
            Content = creationRequest.Content,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }
}