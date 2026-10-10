using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class PaginationRequest
{
    // TODO: Set limit to 20
    // [Range(1, 50)]
    public int Limit { get; set; } = 3;
    public string Cursor { get; set; } = string.Empty;
}