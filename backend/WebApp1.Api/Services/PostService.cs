using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class PostService(
    AppDbContext dbContext, 
    CurrentUserService currentUser, 
    TagService tagService, 
    ImageService imageService,
    ILogger<PostService> logger)
{
    public async Task Create(PostCreationRequest creationRequest, int userId)
    {
        var tags = await tagService.CreateOrGetTagsByName(creationRequest.Tags);

        var post = new Post
        {
            UserId = userId,
            Content = creationRequest.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            Tags = tags.Select(t => new PostTag { Tag = t }).ToList()
        };

        var createdImages = new List<Image>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var order = 0;

            foreach (var file in creationRequest.Images)
            {
                var image = await imageService.CreateAsync(file);
                createdImages.Add(image);

                post.Images.Add(new PostImage
                {
                    Image = image,
                    Order = order++
                });
            }

            dbContext.Posts.Add(post);

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            foreach (var image in createdImages)
            {
                try
                {
                    await imageService.DeleteStorageFileAsync(image);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to delete file {StorageKey} after rollback", image.StorageKey);
                }
            }

            throw;
        }  
    }

    public async Task<PaginationResponse<PostResponse>> GetPosts(PaginationRequest postPagination)
    {
        IQueryable<Post> query = dbContext.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    public async Task<PaginationResponse<PostResponse>> GetPostsByUser(PaginationRequest postPagination, int userId)
    {
        IQueryable<Post> query = dbContext.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    public async Task<PaginationResponse<PostResponse>> GetPostsByTag(PaginationRequest postPagination, int tagId)
    {
        IQueryable<Post> query = dbContext.Posts
            .Where(p => p.Tags.Any(t => t.TagId == tagId))
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    public async Task<PaginationResponse<PostResponse>> GetSavedPosts(PaginationRequest postPagination, int userId)
    {
        IQueryable<Post> query = dbContext.PostSaves
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Post);
        
        return await GetPostsByQuery(query, postPagination);
    }

    public async Task<PaginationResponse<PostResponse>> GetFollowingPosts(PaginationRequest postPagination, int userId)
    {
        IQueryable<Post> query = dbContext.UserFollows
            .Where(x => x.FollowerId == userId)
            .SelectMany(x => x.Following.Posts)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(p => p.Id);
        
        return await GetPostsByQuery(query, postPagination);
    }

    private async Task<PaginationResponse<PostResponse>> GetPostsByQuery(IQueryable<Post> query, PaginationRequest postPagination)
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
                    UserTag = p.User.UserTag,
                    IsFollowedByMe = userId != null 
                        && p.User.Followers.Any(y => y.FollowerId == userId),
                    IsFollowingMe  = userId != null 
                        && p.User.Following.Any(y => y.FollowingId == userId)
                },
                LikesCount = p.Likes.Count,
                IsLikedByMe = userId != null && p.Likes.Any(l => l.UserId == userId),
                Tags = p.Tags.Select(pt => new TagResponse
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name
                }).ToList(),
                IsSavedByMe = userId != null && p.Saves.Any(s => s.UserId == userId),
                Images = p.Images.OrderBy(i => i.Order).Select(i => new ImageResponse
                {
                    Id = i.Image.Id,
                    Url = $"/api/images/{i.Image.Id}",
                    Width = i.Image.Width,
                    Height = i.Image.Height
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

    public async Task SavePost(int postId, int userId)
    {
        var alreadySaved = await dbContext.PostSaves
            .AnyAsync(x => x.PostId == postId && x.UserId == userId);

        if (!alreadySaved)
        {
            dbContext.PostSaves.Add(new PostSave
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UnsavePost(int postId, int userId)
    {
        var savedPost = await dbContext.PostSaves
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

        if (savedPost is not null)
        {
            dbContext.PostSaves.Remove(savedPost);
            await dbContext.SaveChangesAsync();
        }
    }
}