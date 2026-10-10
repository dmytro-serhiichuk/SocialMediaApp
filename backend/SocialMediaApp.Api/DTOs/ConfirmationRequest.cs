using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class ConfirmationRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}