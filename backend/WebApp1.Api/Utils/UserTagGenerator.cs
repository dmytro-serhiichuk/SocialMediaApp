using System.Security.Cryptography;
using System.Text;
using WebApp1.Api.Validation;

namespace WebApp1.Api.Utils;

public static class UserTagGenerator
{
    private const string SuffixChars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int SuffixLength = 6;

    public static string GenerateTag(string? name)
    {
        var baseTag = NormalizeForTag(name);

        if (string.IsNullOrEmpty(baseTag)) baseTag = "user";

        var suffix = GenerateSuffix();

        return UserTagRules.Normalize($"{baseTag}_{suffix}");
    }

    public static string NormalizeForTag(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        value = UserTagRules.Normalize(value);

        var builder = new StringBuilder(value.Length);
        var previousWasUnderscore = false;

        foreach (var character in value)
        {
            if (character is >= 'a' and <= 'z' || character is >= '0' and <= '9')
            {
                builder.Append(character);
                previousWasUnderscore = false;
            }
            else if (!previousWasUnderscore)
            {
                builder.Append('_');
                previousWasUnderscore = true;
            }
        }

        return builder.ToString().Trim('_');
    }

    private static string GenerateSuffix()
    {
        Span<char> result = stackalloc char[SuffixLength];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SuffixChars[RandomNumberGenerator.GetInt32(SuffixChars.Length)];
        }

        return new string(result);
    }
}