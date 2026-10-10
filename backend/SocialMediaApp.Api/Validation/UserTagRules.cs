using System.Text.RegularExpressions;

namespace SocialMediaApp.Api.Validation;

public static class UserTagRules
{
    public const int MaxLength = 50;
    public const int MinLength = 3;
    private static readonly Regex TagRegex = new(@"^[a-z0-9_]+$", RegexOptions.Compiled);

    public const string ErrorMessage = "UserTag is too long (maximum 50 characters) or contains invalid characters (only Latin letters, numbers and '_' are allowed).";

    public static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    public static bool IsValid(string value)
    {
        return value.Length >= MinLength && value.Length <= MaxLength && TagRegex.IsMatch(value);
    }
}