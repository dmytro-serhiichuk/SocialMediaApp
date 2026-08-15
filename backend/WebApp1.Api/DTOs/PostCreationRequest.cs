using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class PostCreationRequest
{
    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;
}