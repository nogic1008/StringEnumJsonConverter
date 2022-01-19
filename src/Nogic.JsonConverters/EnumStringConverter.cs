namespace Nogic.JsonConverters;

/// <summary>Converter to convert enums to and from strings.</summary>
public class EnumStringConverter : JsonConverterFactory
{
    /// <summary>
    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" path="/param[@name='allowIntegerValues']"/>
    /// </summary>
    private readonly bool _allowIntegerValues;

    /// <summary>
    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" path="/param[@name='namingPolicy']"/>
    /// </summary>
    private readonly JsonNamingPolicy? _namingPolicy;

    /// <summary>Constractor.</summary>
    /// <param name="allowIntegerValues">
    /// True to allow undefined <see langword="enum"/> values. When <see langword="true"/>, if an <see langword="enum"/> value isn't
    /// defined it will output as a number rather than a <see langword="string"/>.
    /// </param>
    /// <param name="namingPolicy">Naming policy of <see langword="enum"/> strings.</param>
    public EnumStringConverter(bool allowIntegerValues = true, JsonNamingPolicy? namingPolicy = null)
        => (_allowIntegerValues, _namingPolicy) = (allowIntegerValues, namingPolicy);

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(
            typeof(EnumStringConverter<>).MakeGenericType(typeToConvert),
            new object?[] { _allowIntegerValues, _namingPolicy, options });
}
