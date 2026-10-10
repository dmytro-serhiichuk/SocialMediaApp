using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Utils;

public record SessionCreationResult(
    string Token,
    Session Session);