using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Exceptions;
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
    public async Task Create(PostUpdateRequest creationRequest, int userId)
    {
        if (creationRequest.ImageInfos.Count != creationRequest.Images.Count)
        {
            throw new InvalidPostImagesException();
        }

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
            for (var i = 0; i < creationRequest.ImageInfos.Count; i++)
            {
                var index = creationRequest.ImageInfos[i].Index ?? throw new InvalidPostImagesException();
                var file = creationRequest.Images[index];

                var image = await imageService.CreateAsync(file);
                createdImages.Add(image);

                post.Images.Add(new PostImage
                {
                    Image = image,
                    Order = i
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

    public async Task<bool> UpdatePost(PostUpdateRequest updateRequest, int userId, int postId)
    {
        // TODO: check if data was actually changed
        var existingImageIds = updateRequest.ImageInfos
            .Where(x => x.ImageId is not null)
            .Select(x => x.ImageId!.Value)
            .ToHashSet();

        var post = await dbContext.Posts
            .Include(x => x.Images)
            .ThenInclude(x => x.Image)
            .Include(x => x.Tags)
            .Where(x => x.UserId == userId && x.Id == postId)
            .SingleOrDefaultAsync();

        if (post is null) return false;

        var currentImagesById = post.Images.ToDictionary(x => x.ImageId);

        if (existingImageIds.Any(id => !currentImagesById.ContainsKey(id)))
            return false;

        var imagesToRemove = post.Images
            .Where(x => !existingImageIds.Contains(x.ImageId))
            .ToList();

        var tags = await tagService.CreateOrGetTagsByName(updateRequest.Tags);

        var newlyCreatedImages = new List<Image>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var orderedImages = new List<Image>();

            foreach (var info in updateRequest.ImageInfos)
            {
                if (info.ImageId is not null)
                {
                    orderedImages.Add(currentImagesById[info.ImageId.Value].Image);
                }
                else
                {
                    var file = updateRequest.Images[info.Index!.Value];
                    var image = await imageService.CreateAsync(file);
                    newlyCreatedImages.Add(image);
                    orderedImages.Add(image);
                }
            }

            dbContext.RemoveRange(post.Images);
            post.Images.Clear();

            dbContext.RemoveRange(imagesToRemove.Select(x => x.Image));

            var newTagIds = tags.Select(t => t.Id).ToHashSet();
            foreach (var oldTag in post.Tags.Where(x => !newTagIds.Contains(x.TagId)).ToList())
            {
                post.Tags.Remove(oldTag);
            }
            
            post.Content = updateRequest.Content;
            post.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            for (var i = 0; i < orderedImages.Count; i++)
            {
                post.Images.Add(new PostImage
                {
                    Image = orderedImages[i],
                    Order = i
                });
            }

            var currentTagIds = post.Tags.Select(x => x.TagId).ToHashSet();
            foreach (var tag in tags.Where(t => !currentTagIds.Contains(t.Id)))
            {
                post.Tags.Add(new PostTag { Tag = tag });
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            foreach (var image in newlyCreatedImages)
            {
                try
                {
                    await imageService.DeleteStorageFileAsync(image);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to delete file {StorageKey} after rollback", image.StorageKey);
                }
            }

            throw;
        }

        foreach (var removedPostImage in imagesToRemove)
        {
            try
            {
                await imageService.DeleteStorageFileAsync(removedPostImage.Image);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete file {StorageKey} for removed image", removedPostImage.Image.StorageKey);
            }
        }

        return true;
    }

    public async Task<PostResponse?> GetPostById(int id)
    {
        int? userId = currentUser.UserId;

        return await dbContext.Posts
            .Where(p => p.Id == id)
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
                    Url = ImageResponse.GetApiURL(i.ImageId),
                    Width = i.Image.Width,
                    Height = i.Image.Height
                }).ToList(),
                CommentsCount = p.Comments.Count
            })
            .SingleOrDefaultAsync();
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
                    Url = ImageResponse.GetApiURL(i.ImageId),
                    Width = i.Image.Width,
                    Height = i.Image.Height
                }).ToList(),
                CommentsCount = p.Comments.Count
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

    public async Task<PostEditInfoResponse?> GetPostEditInfo(int postId, int userId)
    {
        return await dbContext.Posts
            .Where(x => x.Id == postId && x.UserId == userId)
            .Select(x => new PostEditInfoResponse
            {
                Id = x.Id,
                Content = x.Content,
                Tags = x.Tags.Select(x => x.Tag.Name).ToList(),
                Images = x.Images
                    .OrderBy(i => i.Order)
                    .Select(i => new ImageResponse
                    {
                        Id = i.Image.Id,
                        Url = ImageResponse.GetApiURL(i.ImageId),
                        Width = i.Image.Width,
                        Height = i.Image.Height
                    }).ToList()
            }).SingleOrDefaultAsync();
    }
}