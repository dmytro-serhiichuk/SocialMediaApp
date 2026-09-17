using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApp1.Api.Validation;

namespace WebApp1.Api.DTOs;

public class PostUpdateRequest : IValidatableObject
{
    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = null!;
    public List<string> Tags { get; set; } = [];
    public List<IFormFile> Images { get; set; } = [];
    [Required]
    public string Info { get; set; } = null!;
    public List<InputImageInfoRequest> ImageInfos { get; private set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Tags = Tags
            .Select(TagRules.Normalize)
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();

        if (Tags.Count > 5)
        {
            yield return new ValidationResult(
                "Only up to 5 tags are available for a post",
                [nameof(Tags)]);
        }

        foreach (var tag in Tags)
        {
            if (!TagRules.IsValid(tag))
            {
                yield return new ValidationResult(
                    $"Tag '{tag}' is too long (maximum 32 characters) or contains invalid characters (only Latin letters, numbers and '_' are allowed).",
                    [nameof(Tags)]);
            }
        }

        if (!TryDeserializeImageInfo(out var imageInfos))
        {
             yield return new ValidationResult(
                "Invalid image information.",
                [nameof(Info)]);

            yield break;
        }

        ImageInfos = imageInfos;

        if (ImageInfos.Count is < 1 or > 10 || Images.Count > 10)
        {
            yield return new ValidationResult(
                "Post must have between 1 and 10 images",
                [nameof(Info)]);

            yield break;
        }

        foreach (var imageInfo in ImageInfos)
        {
            if ((imageInfo.ImageId is not null && imageInfo.Index is not null) ||
                (imageInfo.ImageId is null && imageInfo.Index is null))
            {
                yield return new ValidationResult(
                    "Invalid image information.",
                    [nameof(Info)]);

                yield break;
            }
        }

        var newImageIndices = ImageInfos
            .Where(x => x.Index is not null)
            .Select(x => x.Index!.Value)
            .ToList();

        if (newImageIndices.Count != Images.Count ||
            newImageIndices.Any(x => x < 0 || x >= Images.Count) || 
            newImageIndices.Distinct().Count() != newImageIndices.Count)
        {
            yield return new ValidationResult(
                "Invalid image information.",
                [nameof(Info)]);

            yield break;
        }
    }

    private bool TryDeserializeImageInfo(out List<InputImageInfoRequest> imageInfos)
    {
        try
        {
            imageInfos = JsonSerializer.Deserialize<List<InputImageInfoRequest>>(Info) ?? [];
            return true;
        }
        catch (JsonException)
        {
            imageInfos = [];
            return false;
        }
    }
}