using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class EmailChangeRequest
{
    [Required]
    public string NewEmail { get; set; } = string.Empty;
}