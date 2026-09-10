using WebApp1.Api.Models;

namespace WebApp1.Api.Utils;

public record ExternalLoginInfo(
    ExternalLoginProvider Provider,
    string ProviderUserId,
    string Email,
    string Name
);