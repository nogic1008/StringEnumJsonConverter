#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="DateOnly"/>.</summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    /// <summary>
    /// <inheritdoc cref="DateOnlyConverter(string, IFormatProvider?)" path="/param[@name='serializationFormat']"/>
    /// </summary>
    private readonly string _serializationFormat;

    /// <summary>
    /// <inheritdoc cref="DateOnlyConverter(string, IFormatProvider?)" path="/param[@name='provider']"/>
    /// </summary>
    private readonly IFormatProvider? _provider;

    /// <summary>Initializes a new instance of <see cref="DateOnlyConverter"/>.</summary>
    /// <param name="serializationFormat">A standard or custom date format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public DateOnlyConverter(string serializationFormat = "yyyy-MM-dd", IFormatProvider? provider = null)
        => (_serializationFormat, _provider) = (serializationFormat, provider);

    /// <inheritdoc/>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.ParseExact(reader.GetString()!, _serializationFormat, _provider);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat, _provider));
}
#endif
