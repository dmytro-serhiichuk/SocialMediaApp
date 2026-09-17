using System.Text.Json.Serialization;

namespace WebApp1.Api.DTOs;

public class InputImageInfoRequest
{
    [JsonPropertyName("imageId")]
    public Guid? ImageId { get; set; }
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}