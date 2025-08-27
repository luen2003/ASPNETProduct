using System.Text.Json.Serialization;
using IdentityServer.Utils;

namespace IdentityServer.Extensions;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.FromDateTime(reader.GetDateTime());
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString(Constant.UtcDateFormat);
        writer.WriteStringValue(isoDate);
    }
}