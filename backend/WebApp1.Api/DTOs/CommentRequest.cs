using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class CommentRequest
{
    [Required]
    [MaxLength(2048)]
    public string Content { get; set; } = null!;
}