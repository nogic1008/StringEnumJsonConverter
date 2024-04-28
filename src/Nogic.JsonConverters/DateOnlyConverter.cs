namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="DateOnly"/>.</summary>
/// <param name="serializationFormat">A standard or custom date format string.</param>
/// <param name="provider">An object that supplies culture-specific formatting information.</param>
#if NET7_0_OR_GREATER
[Obsolete("Use internal System.Text.Json.Serialization.Converters.DateOnlyConverter instead.")]
#endif
public class DateOnlyConverter(string serializationFormat = "yyyy-MM-dd", IFormatProvider? provider = null) : JsonConverter<DateOnly>
{
    /// <inheritdoc/>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.ParseExact(reader.GetString()!, serializationFormat, provider);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat, provider));
}
