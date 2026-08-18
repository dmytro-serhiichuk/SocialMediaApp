using System.ComponentModel.DataAnnotations;
using WebApp1.Api.Validation;

namespace WebApp1.Api.DTOs;

public class TagSearchRequest : IValidatableObject
{
    public string Query { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!TagRules.IsValid(Query))
        {
            yield return new ValidationResult(
                $"Query is too long (maximum 32 characters) or contains invalid characters (only Latin letters, numbers and '_' are allowed).",
                [nameof(Query)]);
        }
    }
}