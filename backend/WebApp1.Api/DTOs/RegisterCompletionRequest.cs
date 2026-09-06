using System.ComponentModel.DataAnnotations;
using WebApp1.Api.Validation;

namespace WebApp1.Api.DTOs;

public class RegisterCompletionRequest : IValidatableObject
{
    [Required]
    public string UserTag { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;

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