using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class ConfirmationRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}