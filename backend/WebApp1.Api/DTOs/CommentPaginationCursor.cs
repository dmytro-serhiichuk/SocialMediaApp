namespace WebApp1.Api.DTOs;

public record CommentPaginationCursor(
    int Id,
    DateTimeOffset CreatedAt
);