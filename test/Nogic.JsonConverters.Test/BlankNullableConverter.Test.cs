namespace Nogic.JsonConverters.Test;

/// <summary>
/// Test for <see cref="BlankNullableConverterFactory"/> and <see cref="BlankNullableConverter{T}"/>.
/// </summary>
public sealed class BlankNullableConverterTest
{
    private static readonly JsonSerializerOptions _options = new()
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

    /// <summary>Test data for <see cref="CanSerializeJson"/></summary>
    public static IEnumerable<object[]> TestData_CanSerializeJson => new[]
    {
        new object[]{ 1, "1" },
        new object[]{ TypeCode.Decimal, "15" },
        new object[]{ new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero), "\"2022-01-26T10:00:27+00:00\"" },
    };
    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Write"/> writes expected JSON string.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/></typeparam>
    /// <param name="value">serialize target</param>
    /// <param name="expectedJson">expected JSON string</param>
    [Theory]
    [MemberData(nameof(TestData_CanSerializeJson))]
    public void CanSerializeJson<T>(T value, string expectedJson) where T : struct
    {
        _ = JsonSerializer.Serialize(new T?(), _options).Should().Be("null");
        _ = JsonSerializer.Serialize<T?>(value, _options).Should().Be(expectedJson);
    }

    /// <summary>Test data for <see cref="CanDeserializeJson"/></summary>
    public static IEnumerable<object[]> TestData_CanDeserializeJson => new[]
    {
        new object[]{ "1", 1 },
        new object[]{ "3", TypeCode.Boolean },
        new object[]{ "\"2022-01-26T10:00:27+00:00\"", new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero) },
    };
    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Read"/> returns expected value.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="expected"/></typeparam>
    /// <param name="json">JSON string for deserialize</param>
    /// <param name="expected">expected value</param>
    [Theory]
    [MemberData(nameof(TestData_CanDeserializeJson))]
    public void CanDeserializeJson<T>(string json, T expected) where T : struct
    {
        _ = JsonSerializer.Deserialize<T?>("null", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<T?>("\"\"", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<T?>(json, _options).Should().Be(expected);
    }
}
