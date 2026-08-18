using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WebApp1.Api.Validation;

namespace WebApp1.Api.DTOs;

public class PostCreationRequest : IValidatableObject
{
    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Tags = Tags
            .Select(t => TagRules.Normalize(t))
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
            if (!TagRules.IsValid(tag))
            {
                yield return new ValidationResult(
                    $"Tag '{tag}' is too long (maximum 32 characters) or contains invalid characters (only Latin letters, numbers and '_' are allowed).",
                    [nameof(Tags)]);
            }
        }
    }
}