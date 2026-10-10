namespace SocialMediaApp.Api.DTOs;

public record PostPaginationCursor(
    int Id,
    DateTimeOffset CreatedAt
);