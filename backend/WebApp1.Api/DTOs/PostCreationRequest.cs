using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApp1.Api.DTOs;

public class PostCreationRequest : IValidatableObject
{
    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    private static readonly Regex TagRegex = new(@"^[a-z0-9_]+$", RegexOptions.Compiled);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Tags = Tags
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();

        if (Tags.Count > 5)
        {
            yield return new ValidationResult(
                "Only up to 5 tags are available for a post",
                [nameof(Tags)]);
        }

        foreach (var tag in Tags)
        {
            if (tag.Length > 32)
            {
                yield return new ValidationResult(
                    $"Tag '{tag}' is too long (maximum 64 characters).",
                    [nameof(Tags)]);
            }
            else if (!TagRegex.IsMatch(tag))
            {
                yield return new ValidationResult(
                    $"Tag '{tag}' contains invalid characters. Only Latin letters, numbers and '_' are allowed.",
                    [nameof(Tags)]);
            }
        }
    }
}