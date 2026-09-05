using WebApp1.Api.Models;

namespace WebApp1.Api.Utils;

public record SessionCreationResult(
    string Token,
    Session Session);