using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace WebApp1.Api.Utils;

public static class CursorEncoder
{
    private static readonly JsonSerializerOptions jsonOptions =
        new(JsonSerializerDefaults.Web);

    public static string Encode<T>(T cursor)
    {
        var json = JsonSerializer.Serialize(cursor, jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        return WebEncoders.Base64UrlEncode(bytes);
    }

    public static T? Decode<T>(string cursor)
    {
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(cursor);
            var json = Encoding.UTF8.GetString(bytes);

            return JsonSerializer.Deserialize<T>(json, jsonOptions);
        }
        catch (FormatException)
        {
            return default;
        }
        catch (JsonException)
        {
            return default;
        }
    }
}