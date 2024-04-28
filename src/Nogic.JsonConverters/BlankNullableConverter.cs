namespace Nogic.JsonConverters;

/// <inheritdoc cref="BlankNullableConverterFactory" />
/// <typeparam name="T">struct value type</typeparam>
/// <param name="converter">
/// Json converter for <typeparamref name="T"/> that uses non-null value.
/// </param>
public class BlankNullableConverter<T>(JsonConverter<T> converter) : JsonConverter<T?> where T : struct
{
    /// <inheritdoc/>
    public override bool HandleNull => true;

    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType != JsonTokenType.Null
        && (reader.TokenType != JsonTokenType.String || !string.IsNullOrEmpty(reader.GetString()))
            ? converter.Read(ref reader, typeof(T), options)
            : null;

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteStringValue(string.Empty);
            return;
        }
        converter.Write(writer, value.GetValueOrDefault(), options);
    }
}
