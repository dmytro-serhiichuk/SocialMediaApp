using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class EmailChangeRequest
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = string.Empty;
}