using System.ComponentModel.DataAnnotations;
using SocialMediaApp.Api.Validation;

namespace SocialMediaApp.Api.DTOs;

public class RegisterRequest : IValidatableObject
{
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string UserTag { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    // TODO: add password validation
    public string Password { get; set; } = string.Empty;

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