using System.Text.RegularExpressions;

namespace WebApp1.Api.Validation;

public static class UserTagRules
{
    public const int MaxLength = 50;
    public const int MinLength = 3;
    private static readonly Regex TagRegex = new(@"^[a-z0-9_]+$", RegexOptions.Compiled);

    public static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    public static bool IsValid(string value)
    {
        return value.Length >= MinLength && value.Length <= MaxLength && TagRegex.IsMatch(value);
    }
}