using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class RegisterRequest
{
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string UserTag { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}