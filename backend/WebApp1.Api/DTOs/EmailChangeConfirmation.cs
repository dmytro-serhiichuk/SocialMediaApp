using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class EmailChangeConfirmation
{
    [Required]
    public string Token { get; set; } = string.Empty;
}