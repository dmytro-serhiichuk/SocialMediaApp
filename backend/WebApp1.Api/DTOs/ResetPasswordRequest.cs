using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    // TODO: add password validation
    public string NewPassword { get; set; } = string.Empty;
}