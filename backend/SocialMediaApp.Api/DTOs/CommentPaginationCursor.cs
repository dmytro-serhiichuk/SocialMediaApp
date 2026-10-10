namespace SocialMediaApp.Api.DTOs;

public record CommentPaginationCursor(
    int Id,
    DateTimeOffset CreatedAt
);