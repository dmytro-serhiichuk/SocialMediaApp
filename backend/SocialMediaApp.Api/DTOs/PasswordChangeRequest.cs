using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class PasswordChangeRequest
{
    // TODO: add password validation
    [Required]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}