using System.Text.Json.Serialization;

namespace SocialMediaApp.Api.DTOs;

public class InputImageInfoRequest
{
    [JsonPropertyName("imageId")]
    public Guid? ImageId { get; set; }
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}