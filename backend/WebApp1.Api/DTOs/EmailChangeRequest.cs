using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class EmailChangeRequest
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = string.Empty;
}