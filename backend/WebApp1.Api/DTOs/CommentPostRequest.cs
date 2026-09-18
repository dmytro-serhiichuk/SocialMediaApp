using System.ComponentModel.DataAnnotations;

namespace WebApp1.Api.DTOs;

public class CommentPostRequest
{
    [Required]
    [MaxLength(2048)]
    public string Content { get; set; } = null!;
}