#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="TimeOnly"/>.</summary>
public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    /// <summary>
    /// <inheritdoc cref="TimeOnlyConverter(string, IFormatProvider?)" path="/param[@name='serializationFormat']"/>
    /// </summary>
    private readonly string _serializationFormat;

    /// <summary>
    /// <inheritdoc cref="TimeOnlyConverter(string, IFormatProvider?)" path="/param[@name='provider']"/>
    /// </summary>
    private readonly IFormatProvider? _provider;

    /// <summary>Initializes a new instance of <see cref="TimeOnlyConverter"/>.</summary>
    /// <param name="serializationFormat">A standard or custom time format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public TimeOnlyConverter(string serializationFormat = "HH:mm:ss.fff", IFormatProvider? provider = null)
        => (_serializationFormat, _provider) = (serializationFormat, provider);

    /// <inheritdoc/>
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TimeOnly.ParseExact(reader.GetString()!, _serializationFormat, _provider);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat, _provider));
}
#endif
