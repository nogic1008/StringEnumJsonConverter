namespace Nogic.JsonConverters;

/// <summary>Converter to convert enums to and from strings.</summary>
/// <param name="allowIntegerValues">
/// True to allow undefined <see langword="enum"/> values.
/// When <see langword="true"/>, if an <see langword="enum"/> value isn't defined it will output as a number rather than a <see langword="string"/>.
/// </param>
/// <param name="namingPolicy">Naming policy of <see langword="enum"/> strings.</param>
public class EnumStringConverterFactory(bool allowIntegerValues = true, JsonNamingPolicy? namingPolicy = null) : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions? options)
        => (JsonConverter?)Activator.CreateInstance(typeof(EnumStringConverter<>).MakeGenericType(typeToConvert), [allowIntegerValues, namingPolicy, options]);
}
