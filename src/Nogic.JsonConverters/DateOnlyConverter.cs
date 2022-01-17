#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="DateOnly"/>.</summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string _serializationFormat;

    public DateOnlyConverter(string serializationFormat = "yyyy-MM-dd")
        => _serializationFormat = serializationFormat;

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.ParseExact(reader.GetString()!, _serializationFormat);

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}
#endif
