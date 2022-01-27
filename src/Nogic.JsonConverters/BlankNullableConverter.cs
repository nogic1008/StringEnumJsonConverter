namespace Nogic.JsonConverters;

/// <inheritdoc cref="BlankNullableConverterFactory" />
/// <typeparam name="T">struct value type</typeparam>
public class BlankNullableConverter<T> : JsonConverter<T?> where T : struct
{
    /// <summary>
    /// <inheritdoc cref="BlankNullableConverter(JsonConverter{T})" path="/param[@name='converter']"/>
    /// </summary>
    private readonly JsonConverter<T> _converter;

    /// <summary>Initializes a new instance of <see cref="BlankNullableConverter{T}"/>.</summary>
    /// <param name="converter">
    /// Json converter for <typeparamref name="T"/> that uses non-null value.
    /// </param>
    public BlankNullableConverter(JsonConverter<T> converter) => _converter = converter;

    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType != JsonTokenType.Null
        && (reader.TokenType != JsonTokenType.String || !string.IsNullOrEmpty(reader.GetString()))
            ? _converter.Read(ref reader, typeof(T), options)
            : null;

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
            writer.WriteStringValue("");
        else
            _converter.Write(writer, value.Value, options);
    }
}
