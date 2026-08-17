using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class PostService(AppDbContext dbContext, CurrentUserService currentUser, TagService tagService)
{
    public async Task Create(PostCreationRequest creationRequest, int userId)
    {
        var tags = await tagService.CreateOrGetTagsByName(creationRequest.Tags);

        dbContext.Posts.Add(new Post
        {
            UserId = userId,
            Content = creationRequest.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            Tags = tags.Select(t => new PostTag { Tag = t }).ToList()
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<PaginationResponse<PostResponse>> GetPosts(PostPaginationRequest postPagination)
    {
        IQueryable<Post>? query = dbContext.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    public async Task<PaginationResponse<PostResponse>> GetPostsByUser(PostPaginationRequest postPagination, int userId)
    {
        IQueryable<Post>? query = dbContext.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    private async Task<PaginationResponse<PostResponse>> GetPostsByQuery(IQueryable<Post> query, PostPaginationRequest postPagination)
    {
        var cursor = CursorEncoder.Decode<PostPaginationCursor>(postPagination.Cursor);
        
        if (cursor is not null)
        {
            query = query.Where(p => 
                p.CreatedAt < cursor.CreatedAt || 
                (p.CreatedAt == cursor.CreatedAt && p.Id < cursor.Id)
            );
        }

        int? userId = currentUser.UserId;

        var posts = await query
            .Take(postPagination.Limit + 1)
            .Select(p => new PostResponse
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                User = new UserResponse
                {
                    Id = p.User.Id,
                    UserName = p.User.UserName,
                    UserTag = p.User.UserTag
                },
                LikesCount = p.Likes.Count,
                IsLikedByMe = userId != null && p.Likes.Any(l => l.UserId == userId),
                Tags = p.Tags.Select(pt => new TagResponse
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name
                }).ToList()
            })
            .ToListAsync();

        var hasMore = posts.Count > postPagination.Limit;
        if (hasMore) posts.RemoveAt(posts.Count - 1);

        string? nextCursor = null;
        if (hasMore && posts.Count > 0)
        {
            var last = posts[^1];
            nextCursor = CursorEncoder.Encode(new PostPaginationCursor
            (
                last.Id,
                last.CreatedAt
            ));
        }

        return new PaginationResponse<PostResponse>
        {
            Items = posts,
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }

    public async Task LikePost(int postId, int userId)
    {
        var alreadyLiked = await dbContext.PostLikes
            .AnyAsync(x => x.PostId == postId && x.UserId == userId);

        if (!alreadyLiked)
        {
            dbContext.PostLikes.Add(new PostLike
            {
                PostId = postId,
                UserId = userId
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UnlikePost(int postId, int userId)
    {
        var like = await dbContext.PostLikes
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

        if (like is not null)
        {
            dbContext.PostLikes.Remove(like);
            await dbContext.SaveChangesAsync();
        }
    }
}