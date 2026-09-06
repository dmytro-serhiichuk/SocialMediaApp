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
                UserTagRules.ErrorMessage,
                [nameof(UserTag)]);
        }
    }
}