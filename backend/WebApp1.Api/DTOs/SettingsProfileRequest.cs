using System.ComponentModel.DataAnnotations;
using WebApp1.Api.Validation;

namespace WebApp1.Api.DTOs;

public class SettingsProfileRequest : IValidatableObject
{
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string UserTag { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!UserTagRules.IsValid(UserTag))
        {
            yield return new ValidationResult(
                $"UserTag is too long (maximum 50 characters) or contains invalid characters (only Latin letters, numbers and '_' are allowed).",
                [nameof(UserTag)]);
        }
    }
}