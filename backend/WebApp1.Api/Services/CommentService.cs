using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class CommentService(
    AppDbContext dbContext,
    CurrentUserService currentUser)
{
    public async Task<CommentResponse?> CommentPost(CommentRequest request, int userId, int postId)
    {
        var postExist = dbContext.Posts
            .Any(x => x.Id == postId);

        if (!postExist) return null;

        var user = await dbContext.Users
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        if (user is null) return null;

        var comment = new Comment
        {
            UserId = userId,
            PostId = postId,
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.Comments.Add(comment);

        await dbContext.SaveChangesAsync();

        return new CommentResponse
        {
            Id = comment.Id,
            User = new UserResponse
            {
                Id = userId,
                UserName = user.UserName,
                UserTag = user.UserTag,
                IsFollowedByMe = user.Followers.Any(y => y.FollowerId == userId),
                IsFollowingMe  = user.Following.Any(y => y.FollowingId == userId)
            },
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<CommentResponse?> ReplyComment(CommentRequest request, int userId, int postId, int commentId)
    {
        var repliedComment = await dbContext.Comments
            .Include(x => x.User)
            .Where(x => x.Id == commentId && x.PostId == postId)
            .SingleOrDefaultAsync();

        if (repliedComment is null) return null;

        var user = await dbContext.Users
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        if (user is null) return null;

        var reply = new Comment
        {
            UserId = userId,
            PostId = postId,
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            ReplyToUserId = repliedComment.UserId,
            ParentCommentId = repliedComment.ParentCommentId ?? repliedComment.Id
        };

        dbContext.Comments.Add(reply);

        await dbContext.SaveChangesAsync();

        return new CommentResponse
        {
            Id = reply.Id,
            User = new UserResponse
            {
                Id = userId,
                UserName = user.UserName,
                UserTag = user.UserTag,
                IsFollowedByMe = user.Followers.Any(y => y.FollowerId == userId),
                IsFollowingMe  = user.Following.Any(y => y.FollowingId == userId)
            },
            Content = reply.Content,
            CreatedAt = reply.CreatedAt,
            ReplyToUserTag = repliedComment.User.UserTag
        };
    }

    public async Task<PaginationResponse<CommentResponse>> GetPostComments(PaginationRequest pagination, int postId)
    {
        IQueryable<Comment> query = dbContext.Comments
            .Where(x => x.PostId == postId && x.ParentCommentId == null)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id);

        var cursor = CursorEncoder.Decode<CommentPaginationCursor>(pagination.Cursor);
        
        if (cursor is not null)
        {
            query = query.Where(p => 
                p.CreatedAt < cursor.CreatedAt || 
                (p.CreatedAt == cursor.CreatedAt && p.Id < cursor.Id)
            );
        }

        int? userId = currentUser.UserId;

        var comments = await query
            .Take(pagination.Limit + 1)
            .Select(x => new CommentResponse
            {
                Id = x.Id,
                User = new UserResponse
                {
                    Id = x.UserId,
                    UserName = x.User.UserName,
                    UserTag = x.User.UserTag,
                    IsFollowedByMe = userId != null && 
                        x.User.Followers.Any(y => y.FollowerId == userId),
                    IsFollowingMe  = userId != null &&
                        x.User.Following.Any(y => y.FollowingId == userId)
                },
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                RepliesCount = x.Replies.Count,
                LikesCount = x.Likes.Count,
                IsLikedByMe = userId != null && x.Likes.Any(l => l.UserId == userId),
            }).ToListAsync();

        var hasMore = comments.Count > pagination.Limit;
        if (hasMore) comments.RemoveAt(comments.Count - 1);

        string? nextCursor = null;
        if (hasMore && comments.Count > 0)
        {
            var last = comments[^1];
            nextCursor = CursorEncoder.Encode(new CommentPaginationCursor
            (
                last.Id,
                last.CreatedAt
            ));
        }

        return new PaginationResponse<CommentResponse>
        {
            Items = comments,
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }

    public async Task<PaginationResponse<CommentResponse>> GetCommentReplies(PaginationRequest pagination, int postId, int commentId)
    {
        IQueryable<Comment> query = dbContext.Comments
            .Where(x => x.PostId == postId && x.ParentCommentId == commentId)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id);

        var cursor = CursorEncoder.Decode<CommentPaginationCursor>(pagination.Cursor);
        
        if (cursor is not null)
        {
            query = query.Where(p => 
                p.CreatedAt > cursor.CreatedAt || 
                (p.CreatedAt == cursor.CreatedAt && p.Id < cursor.Id)
            );
        }

        int? userId = currentUser.UserId;

        var comments = await query
            .Take(pagination.Limit + 1)
            .Select(x => new CommentResponse
            {
                Id = x.Id,
                User = new UserResponse
                {
                    Id = x.UserId,
                    UserName = x.User.UserName,
                    UserTag = x.User.UserTag,
                    IsFollowedByMe = userId != null && 
                        x.User.Followers.Any(y => y.FollowerId == userId),
                    IsFollowingMe  = userId != null &&
                        x.User.Following.Any(y => y.FollowingId == userId)
                },
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                ReplyToUserTag = x.ReplyToUser.UserTag,
                LikesCount = x.Likes.Count,
                IsLikedByMe = userId != null && x.Likes.Any(l => l.UserId == userId),
            }).ToListAsync();

        var hasMore = comments.Count > pagination.Limit;
        if (hasMore) comments.RemoveAt(comments.Count - 1);

        string? nextCursor = null;
        if (hasMore && comments.Count > 0)
        {
            var last = comments[^1];
            nextCursor = CursorEncoder.Encode(new CommentPaginationCursor
            (
                last.Id,
                last.CreatedAt
            ));
        }

        return new PaginationResponse<CommentResponse>
        {
            Items = comments,
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }

    public async Task LikeComment(int commentId, int userId)
    {
        var alreadyLiked = await dbContext.CommentLikes
            .AnyAsync(x => x.CommentId == commentId && x.UserId == userId);

        if (!alreadyLiked)
        {
            dbContext.CommentLikes.Add(new CommentLike
            {
                CommentId = commentId,
                UserId = userId
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UnlikeComment(int commentId, int userId)
    {
        var like = await dbContext.CommentLikes
            .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);

        if (like is not null)
        {
            dbContext.CommentLikes.Remove(like);
            await dbContext.SaveChangesAsync();
        }
    }
}