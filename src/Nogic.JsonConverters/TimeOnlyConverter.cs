#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="TimeOnly"/>.</summary>
public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat;
    private readonly IFormatProvider? _provider;

    public TimeOnlyConverter(string serializationFormat = "HH:mm:ss.fff", IFormatProvider? provider = null)
        => (_serializationFormat, _provider) = (serializationFormat, provider);

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TimeOnly.ParseExact(reader.GetString()!, _serializationFormat, _provider);

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat, _provider));
}
#endif
