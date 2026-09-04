using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class PasswordChangeRequest
{
    // TODO: add password validation
    [Required]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}