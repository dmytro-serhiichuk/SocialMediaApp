using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApp1.Api.DTOs;

public class RequestPasswordResetRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public RequestPasswordResetType RequestType { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RequestPasswordResetType
{
    Reset,
    Create
}