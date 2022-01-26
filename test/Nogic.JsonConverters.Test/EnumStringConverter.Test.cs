using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nogic.JsonConverters.Test;

/// <summary>Unit test of <see cref="EnumStringConverter"/></summary>
public sealed class EnumStringConverterTest
{
    /// <summary>
    /// Create <see cref="JsonSerializerOptions"/> that contains <see cref="EnumStringConverter"/>.
    /// </summary>
    /// <param name="allowInteger">
    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" path="/param[@name='allowIntegerValues']"/>
    /// </param>
    /// <param name="useCamelCase">Use <see cref="JsonNamingPolicy.CamelCase"/> or not.</param>
    private static JsonSerializerOptions CreateOption(bool allowInteger, bool useCamelCase)
        => new()
        {
            Converters = { new EnumStringConverter(allowInteger, useCamelCase ? JsonNamingPolicy.CamelCase : null) }
        };

    /// <summary>For <see cref="EnumStringConverter{TEnum}.CanConvert(Type)"/> testing</summary>
    private enum TestEnumOver64
    {
        A1, A2, A3, A4, A5, A6, A7, A8, A9, A0,
        B1, B2, B3, B4, B5, B6, B7, B8, B9, B0,
        C1, C2, C3, C4, C5, C6, C7, C8, C9, C0,
        D1, D2, D3, D4, D5, D6, D7, D8, D9, D0,
        E1, E2, E3, E4, E5, E6, E7, E8, E9, E0,
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F0,
        G1, G2, G3, G4, G5, G6, G7, G8, G9, G0,
    }

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.CanConvert"/> returns <see langword="true"/>.
    /// </summary>
    [Fact]
    public void CanConvert_Returns_True()
        => new EnumStringConverter<TestEnumOver64>().CanConvert(typeof(TestEnumOver64)).Should().BeTrue();

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.CanConvert"/> returns <see langword="false"/>.
    /// </summary>
    /// <param name="type">Type to convert</param>
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(TestForConvert))]
    public void CanConvert_Returns_False(Type type)
        => new EnumStringConverter<TestEnumOver64>().CanConvert(type).Should().BeFalse();

    /// <summary>For JSON serialize & deserialize testing</summary>
    [Flags]
    public enum TestForConvert
    {
        None = 0,
        [EnumMember(Value = "One_one")] OneOne = 1,
        [EnumMember] TwoTwo = 2,
        [JsonPropertyName("foo")] Four = 4,
    }

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Write"/> writes <paramref name="expected"/>.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='allowInteger']"/>
    /// <inheritdoc cref="CreateOption" path="/param[@name='useCamelCase']"/>
    /// <param name="enum">Serialize target</param>
    /// <param name="expected">Expected JSON string</param>
    [Theory]
    // Default
    [InlineData(true, false, (TestForConvert)(-1), "-1")]
    [InlineData(true, false, TestForConvert.None, "\"None\"")]
    [InlineData(true, false, TestForConvert.OneOne, "\"One_one\"")]
    [InlineData(true, false, TestForConvert.TwoTwo, "\"TwoTwo\"")]
    [InlineData(true, false, TestForConvert.OneOne | TestForConvert.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(true, false, TestForConvert.Four, "\"foo\"")]
    // Use JsonNamingPolicy.CamelCase
    [InlineData(true, true, (TestForConvert)(-1), "-1")]
    [InlineData(true, true, TestForConvert.None, "\"none\"")]
    [InlineData(true, true, TestForConvert.OneOne, "\"One_one\"")]
    [InlineData(true, true, TestForConvert.TwoTwo, "\"twoTwo\"")]
    [InlineData(true, true, TestForConvert.OneOne | TestForConvert.TwoTwo, "\"oneOne, twoTwo\"")]
    [InlineData(true, true, TestForConvert.Four, "\"foo\"")]
    // Disallow integer
    [InlineData(false, false, TestForConvert.None, "\"None\"")]
    [InlineData(false, false, TestForConvert.OneOne, "\"One_one\"")]
    [InlineData(false, false, TestForConvert.TwoTwo, "\"TwoTwo\"")]
    [InlineData(false, false, TestForConvert.OneOne | TestForConvert.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(false, false, TestForConvert.Four, "\"foo\"")]
    public void CanSerializeJson(bool allowInteger, bool useCamelCase, TestForConvert @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption(allowInteger, useCamelCase)).Should().Be(expected);

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Write"/> throws error.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='allowInteger']"/>
    /// <inheritdoc cref="CreateOption" path="/param[@name='useCamelCase']"/>
    /// <param name="enum">Serialize target</param>
    [Theory]
    // Disallow integer
    [InlineData(false, false, (TestForConvert)(-1))]
    public void CannotSerializeJson(bool allowInteger, bool useCamelCase, TestForConvert @enum)
    {
        var action = () => JsonSerializer.Serialize(@enum, CreateOption(allowInteger, useCamelCase));
        action.Should().Throw<JsonException>();
    }

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Read"/> returns <paramref name="expected"/>.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='allowInteger']"/>
    /// <inheritdoc cref="CreateOption" path="/param[@name='useCamelCase']"/>
    /// <param name="json">JSON string</param>
    /// <param name="expected">Expected <see langword="enum"/> value</param>
    [Theory]
    // Default
    [InlineData(true, false, "-1", (TestForConvert)(-1))]
    [InlineData(true, false, "\"None\"", TestForConvert.None)]
    [InlineData(true, false, "\"One_one\"", TestForConvert.OneOne)]
    [InlineData(true, false, "\"TwoTwo\"", TestForConvert.TwoTwo)]
    [InlineData(true, false, "\"OneOne, TwoTwo\"", TestForConvert.OneOne | TestForConvert.TwoTwo)]
    [InlineData(true, false, "\"4\"", TestForConvert.Four)]
    // Use JsonNamingPolicy.CamelCase
    [InlineData(true, true, "-1", (TestForConvert)(-1))]
    [InlineData(true, true, "\"none\"", TestForConvert.None)]
    [InlineData(true, true, "\"One_one\"", TestForConvert.OneOne)]
    [InlineData(true, true, "\"TwoTwo\"", TestForConvert.TwoTwo)]
    [InlineData(true, true, "\"OneOne, TwoTwo\"", TestForConvert.OneOne | TestForConvert.TwoTwo)]
    [InlineData(true, true, "\"4\"", TestForConvert.Four)]
    // Disallow integer
    [InlineData(false, false, "\"None\"", TestForConvert.None)]
    [InlineData(false, false, "\"One_one\"", TestForConvert.OneOne)]
    [InlineData(false, false, "\"TwoTwo\"", TestForConvert.TwoTwo)]
    [InlineData(false, false, "\"OneOne, TwoTwo\"", TestForConvert.OneOne | TestForConvert.TwoTwo)]
    [InlineData(false, false, "\"4\"", TestForConvert.Four)]
    public void CanDeserializeJson(bool allowInteger, bool useCamelCase, string json, TestForConvert expected)
        => JsonSerializer.Deserialize<TestForConvert>(json, CreateOption(allowInteger, useCamelCase)).Should().Be(expected);

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Read"/> throws error.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='allowInteger']"/>
    /// <inheritdoc cref="CreateOption" path="/param[@name='useCamelCase']"/>
    /// <param name="json">JSON string</param>
    [Theory]
    // Default
    [InlineData(true, false, "\"\"")]
    [InlineData(true, false, "OneOne")]
    [InlineData(true, false, "twotwo")]
    [InlineData(true, false, "\"One_one, TwoTwo\"")]
    // Use JsonNamingPolicy.CamelCase
    [InlineData(true, true, "\"\"")]
    [InlineData(true, true, "OneOne")]
    [InlineData(true, true, "twotwo")]
    [InlineData(true, true, "\"One_one, TwoTwo\"")]
    // Disallow integer
    [InlineData(false, false, "\"\"")]
    [InlineData(false, false, "-1")]
    [InlineData(false, false, "OneOne")]
    [InlineData(false, false, "twotwo")]
    [InlineData(false, false, "\"One_one, TwoTwo\"")]
    public void CannotDeserializeJson(bool allowInteger, bool useCamelCase, string json)
    {
        var action = () => JsonSerializer.Deserialize<TestForConvert>(json, CreateOption(allowInteger, useCamelCase));
        action.Should().Throw<JsonException>();
    }

    /// <summary>For <see langword="sbyte"/> based testing</summary>
    private enum TestEnumSByte : sbyte { Min = sbyte.MinValue, One = 1, Max = sbyte.MaxValue }
    /// <summary>For <see langword="byte"/> based testing</summary>
    private enum TestEnumByte : byte { Min = byte.MinValue, One = 1, Max = byte.MaxValue }
    /// <summary>For <see langword="short"/> based testing</summary>
    private enum TestEnumInt16 : short { Min = short.MinValue, One = 1, Max = short.MaxValue }
    /// <summary>For <see langword="ushort"/> based testing</summary>
    private enum TestEnumUInt16 : ushort { Min = ushort.MinValue, One = 1, Max = ushort.MaxValue }
    /// <summary>For <see langword="int"/> based testing</summary>
    private enum TestEnumInt32 : int { Min = int.MinValue, One = 1, Max = int.MaxValue }
    /// <summary>For <see langword="uint"/> based testing</summary>
    private enum TestEnumUInt32 : uint { Min = uint.MinValue, One = 1, Max = uint.MaxValue }
    /// <summary>For <see langword="long"/> based testing</summary>
    private enum TestEnumInt64 : long { Min = long.MinValue, One = 1, Max = long.MaxValue }
    /// <summary>For <see langword="ulong"/> based testing</summary>
    private enum TestEnumUInt64 : ulong { Min = ulong.MinValue, One = 1, Max = ulong.MaxValue }

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Write"/> writes <paramref name="expected"/>.
    /// </summary>
    /// <param name="enum">Serialize target</param>
    /// <param name="expected">Expected JSON string</param>
    [Theory]
    [InlineData(TestEnumSByte.Min, "\"Min\"")]
    [InlineData(TestEnumByte.Min, "\"Min\"")]
    [InlineData(TestEnumInt16.Min, "\"Min\"")]
    [InlineData(TestEnumUInt16.Min, "\"Min\"")]
    [InlineData(TestEnumInt32.Min, "\"Min\"")]
    [InlineData(TestEnumUInt32.Min, "\"Min\"")]
    [InlineData(TestEnumInt64.Min, "\"Min\"")]
    [InlineData(TestEnumUInt64.Min, "\"Min\"")]
    [InlineData(TestEnumSByte.One, "\"One\"")]
    [InlineData(TestEnumByte.One, "\"One\"")]
    [InlineData(TestEnumInt16.One, "\"One\"")]
    [InlineData(TestEnumUInt16.One, "\"One\"")]
    [InlineData(TestEnumInt32.One, "\"One\"")]
    [InlineData(TestEnumUInt32.One, "\"One\"")]
    [InlineData(TestEnumInt64.One, "\"One\"")]
    [InlineData(TestEnumUInt64.One, "\"One\"")]
    [InlineData((TestEnumSByte)2, "2")]
    [InlineData((TestEnumByte)2, "2")]
    [InlineData((TestEnumInt16)2, "2")]
    [InlineData((TestEnumUInt16)2, "2")]
    [InlineData((TestEnumInt32)2, "2")]
    [InlineData((TestEnumUInt32)2, "2")]
    [InlineData((TestEnumInt64)2, "2")]
    [InlineData((TestEnumUInt64)2, "2")]
    [InlineData(TestEnumSByte.Max, "\"Max\"")]
    [InlineData(TestEnumByte.Max, "\"Max\"")]
    [InlineData(TestEnumInt16.Max, "\"Max\"")]
    [InlineData(TestEnumUInt16.Max, "\"Max\"")]
    [InlineData(TestEnumInt32.Max, "\"Max\"")]
    [InlineData(TestEnumUInt32.Max, "\"Max\"")]
    [InlineData(TestEnumInt64.Max, "\"Max\"")]
    [InlineData(TestEnumUInt64.Max, "\"Max\"")]
    public void CanSerializeJson_Type(object @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption(true, false)).Should().Be(expected);

    /// <summary>
    /// <see cref="EnumStringConverter{TEnum}.Read"/> returns expected <see langword="enum"/>.
    /// </summary>
    [Fact]
    public void CanDeserializeJson_Type()
    {
        var option = CreateOption(true, false);
        AssertDerialize(sbyte.MinValue.ToString(), TestEnumSByte.Min);
        AssertDerialize(byte.MinValue.ToString(), TestEnumByte.Min);
        AssertDerialize(short.MinValue.ToString(), TestEnumInt16.Min);
        AssertDerialize(ushort.MinValue.ToString(), TestEnumUInt16.Min);
        AssertDerialize(int.MinValue.ToString(), TestEnumInt32.Min);
        AssertDerialize(uint.MinValue.ToString(), TestEnumUInt32.Min);
        AssertDerialize(long.MinValue.ToString(), TestEnumInt64.Min);
        AssertDerialize(ulong.MinValue.ToString(), TestEnumUInt64.Min);
        AssertDerialize("1", TestEnumSByte.One);
        AssertDerialize("1", TestEnumByte.One);
        AssertDerialize("1", TestEnumInt16.One);
        AssertDerialize("1", TestEnumUInt16.One);
        AssertDerialize("1", TestEnumInt32.One);
        AssertDerialize("1", TestEnumUInt32.One);
        AssertDerialize("1", TestEnumInt64.One);
        AssertDerialize("1", TestEnumUInt64.One);
        AssertDerialize(sbyte.MaxValue.ToString(), TestEnumSByte.Max);
        AssertDerialize(byte.MaxValue.ToString(), TestEnumByte.Max);
        AssertDerialize(short.MaxValue.ToString(), TestEnumInt16.Max);
        AssertDerialize(ushort.MaxValue.ToString(), TestEnumUInt16.Max);
        AssertDerialize(int.MaxValue.ToString(), TestEnumInt32.Max);
        AssertDerialize(uint.MaxValue.ToString(), TestEnumUInt32.Max);
        AssertDerialize(long.MaxValue.ToString(), TestEnumInt64.Max);
        AssertDerialize(ulong.MaxValue.ToString(), TestEnumUInt64.Max);

        void AssertDerialize<T>(string json, T expected)
            => JsonSerializer.Deserialize<T>(json, option).Should().Be(expected);
    }
}
