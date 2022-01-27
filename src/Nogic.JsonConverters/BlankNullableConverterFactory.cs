namespace Nogic.JsonConverters;

/// <summary>Json converter for <see cref="Nullable{T}"/> that treats "" as <see langword="null"/>.</summary>
public class BlankNullableConverterFactory : JsonConverterFactory
{
    /// <summary>Cache of <see cref="Nullable{T}"/> type</summary>
    private static readonly Type _nullableType = typeof(Nullable<>);

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == _nullableType;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueTypeToConvert = typeToConvert.GetGenericArguments()[0];
        var valueConverter = options.GetConverter(valueTypeToConvert);

        return (JsonConverter?)Activator.CreateInstance(GetConverterType(valueTypeToConvert), valueConverter);

        static Type GetConverterType(Type valueTypeToConvert)
            => typeof(BlankNullableConverter<>).MakeGenericType(valueTypeToConvert);
    }
}
