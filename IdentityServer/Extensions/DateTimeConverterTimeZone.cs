using System.Globalization;
using System.Text.Json.Serialization;

namespace IdentityServer.Extensions;

public sealed class DateTimeConverterTimeZone : JsonConverter<DateTime>
{
    
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        TimeZoneInfo.ConvertTime(DateTime.Parse(reader.GetString() ?? string.Empty, CultureInfo.CurrentCulture), TimeZoneInfo.Local);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
    }
}