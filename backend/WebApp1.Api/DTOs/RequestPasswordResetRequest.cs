using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class RequestPasswordResetRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}