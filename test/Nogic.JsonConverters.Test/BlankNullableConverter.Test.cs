namespace Nogic.JsonConverters.Test;

/// <summary>
/// Test for <see cref="BlankNullableConverterFactory"/> and <see cref="BlankNullableConverter{T}"/>.
/// </summary>
[TestClass]
public sealed class BlankNullableConverterTest
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new BlankNullableConverterFactory() }
    };

    /// <summary>
    /// When <paramref name="type"/> is <see cref="Nullable{T}"/>, <see cref="BlankNullableConverterFactory.CanConvert"/> returns <see langword="true"/>.
    /// </summary>
    /// <param name="type">Type to convert</param>
    [TestMethod]
    [DataRow(typeof(int?))]
    [DataRow(typeof(TypeCode?))]
    [DataRow(typeof(DateTimeOffset?))]
    public void When_Type_Is_NullableStruct_CanConvert_Returns_True(Type type)
        => new BlankNullableConverterFactory().CanConvert(type).Should().BeTrue();

    /// <summary>
    /// When <paramref name="type"/> is not <see cref="Nullable{T}"/>, <see cref="BlankNullableConverterFactory.CanConvert"/> returns <see langword="false"/>.
    /// </summary>
    /// <param name="type">Type to convert</param>
    [TestMethod]
    [DataRow(typeof(int))]
    [DataRow(typeof(string))]
    [DataRow(typeof(TypeCode))]
    [DataRow(typeof(DateTimeOffset))]
    [DataRow(typeof(Array))]
    [DataRow(typeof(Dictionary<,>))]
    public void When_Type_Is_Not_NullableStruct_CanConvert_Returns_False(Type type)
        => new BlankNullableConverterFactory().CanConvert(type).Should().BeFalse();

    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Write"/> writes expected JSON string.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="value"/></typeparam>
    /// <param name="value">serialize target</param>
    /// <param name="expectedJson">expected JSON string</param>
    [TestMethod]
    public void CanSerializeJson()
    {
        // int
        _ = JsonSerializer.Serialize((int?)null, _options).Should().Be("\"\"");
        _ = JsonSerializer.Serialize(1, _options).Should().Be("1");

        // enum
        _ = JsonSerializer.Serialize((TypeCode?)null, _options).Should().Be("\"\"");
        _ = JsonSerializer.Serialize(TypeCode.Decimal, _options).Should().Be("15");

        // DateTimeOffset
        _ = JsonSerializer.Serialize((DateTimeOffset?)null, _options).Should().Be("\"\"");
        _ = JsonSerializer.Serialize(new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero), _options).Should().Be("\"2022-01-26T10:00:27+00:00\"");
    }

    /// <summary>
    /// <see cref="BlankNullableConverter{T}.Read"/> returns expected value.
    /// </summary>
    /// <typeparam name="T">Type of <paramref name="expected"/></typeparam>
    /// <param name="json">JSON string for deserialize</param>
    /// <param name="expected">expected value</param>
    [TestMethod]
    public void CanDeserializeJson()
    {
        // int
        _ = JsonSerializer.Deserialize<int?>("null", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<int?>("\"\"", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<int?>("1", _options).Should().Be(1);

        // enum
        _ = JsonSerializer.Deserialize<TypeCode?>("null", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<TypeCode?>("\"\"", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<TypeCode?>("3", _options).Should().Be(TypeCode.Boolean);

        // DateTimeOffset
        _ = JsonSerializer.Deserialize<DateTimeOffset?>("null", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<DateTimeOffset?>("\"\"", _options).Should().BeNull();
        _ = JsonSerializer.Deserialize<DateTimeOffset?>("\"2022-01-26T10:00:27+00:00\"", _options).Should().Be(new DateTimeOffset(2022, 1, 26, 10, 0, 27, 0, TimeSpan.Zero));
    }
}
