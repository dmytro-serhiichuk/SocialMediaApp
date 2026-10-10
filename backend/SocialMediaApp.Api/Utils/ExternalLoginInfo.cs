using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Utils;

public record ExternalLoginInfo(
    ExternalLoginProvider Provider,
    string ProviderUserId,
    string Email,
    string Name
);