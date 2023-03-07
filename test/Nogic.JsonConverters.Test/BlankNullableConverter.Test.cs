namespace Nogic.JsonConverters.Test;

/// <summary>
/// Test for <see cref="BlankNullableConverterFactory"/> and <see cref="BlankNullableConverter{T}"/>.
/// </summary>
public sealed class BlankNullableConverterTest
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new BlankNullableConverterFactory() }
    };

    /// <summary>
    /// <see cref="BlankNullableConverterFactory.CanConvert"/> returns <see langword="true"/>.
    /// </summary>
    /// <param name="type">Type to convert</param>
    [Theory]
    [InlineData(typeof(int?))]
    [InlineData(typeof(TypeCode?))]
    [InlineData(typeof(DateTimeOffset?))]
    public void CanConvert_Returns_True(Type type)
        => new BlankNullableConverterFactory().CanConvert(type).Should().BeTrue();

    /// <summary>
    /// <see cref="BlankNullableConverterFactory.CanConvert"/> returns <see langword="false"/>.
    /// </summary>
    /// <param name="type">Type to convert</param>
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(TypeCode))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(Array))]
    [InlineData(typeof(Dictionary<,>))]
    public void CanConvert_Returns_False(Type type)
        => new BlankNullableConverterFactory().CanConvert(type).Should().BeFalse();

    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Write"/> writes expected string.
    /// </summary>
    [Fact]
    public void CanSerializeJson()
    {
        AssertSerialize(1, "1");
        AssertSerialize(TypeCode.Decimal, "15");
        AssertSerialize(new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero), "\"2022-01-26T10:00:27+00:00\"");

        void AssertSerialize<T>(T value, string expected) where T : struct
        {
            _ = JsonSerializer.Serialize<T?>(new T?(), _options).Should().Be("null");
            _ = JsonSerializer.Serialize<T?>(value, _options).Should().Be(expected);
        }
    }

    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Read"/>
    /// </summary>
    [Fact]
    public void CanDeserializeJson()
    {
        AssertDeserialize("1", 1);
        AssertDeserialize("3", TypeCode.Boolean);
        AssertDeserialize("\"2022-01-26T10:00:27+00:00\"", new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero));

        void AssertDeserialize<T>(string json, T expected) where T : struct
        {
            _ = JsonSerializer.Deserialize<T?>("null", _options).Should().BeNull();
            _ = JsonSerializer.Deserialize<T?>("\"\"", _options).Should().BeNull();
            _ = JsonSerializer.Deserialize<T?>(json, _options).Should().Be(expected);
        }
    }
}
