namespace Nogic.JsonConverters.Test;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>Unit test of <see cref="EnumStringConverter"/></summary>
public sealed class EnumStringConverterTest
{
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

    [Fact]
    public void CanConvert_Returns_True()
    {
        // Arrange
        var converter = new EnumStringConverter<TestEnumOver64>(true, null, new());

        // Act - Assert
        converter.CanConvert(typeof(TestEnumOver64)).Should().BeTrue();
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(TestEnum))]
    public void CanConvert_Returns_False(Type type)
    {
        // Arrange
        var converter = new EnumStringConverter<TestEnumOver64>(true, null, new());

        // Act - Assert
        converter.CanConvert(type).Should().BeFalse();
    }

    /// <summary>For JSON serialize & deserialize testing</summary>
    [Flags]
    public enum TestEnum
    {
        None = 0,
        [EnumMember(Value = "One_one")] OneOne = 1,
        [EnumMember] TwoTwo = 2,
        [EnumMember(Value = "foo")] Four = 4,
    }

    [Theory]
    // Default
    [InlineData(true, false, (TestEnum)(-1), "-1")]
    [InlineData(true, false, TestEnum.None, "\"None\"")]
    [InlineData(true, false, TestEnum.OneOne, "\"One_one\"")]
    [InlineData(true, false, TestEnum.TwoTwo, "\"TwoTwo\"")]
    [InlineData(true, false, TestEnum.OneOne | TestEnum.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(true, false, TestEnum.Four, "\"foo\"")]
    // Use JsonNamingPolicy.CamelCase
    [InlineData(true, true, (TestEnum)(-1), "-1")]
    [InlineData(true, true, TestEnum.None, "\"none\"")]
    [InlineData(true, true, TestEnum.OneOne, "\"One_one\"")]
    [InlineData(true, true, TestEnum.TwoTwo, "\"twoTwo\"")]
    [InlineData(true, true, TestEnum.OneOne | TestEnum.TwoTwo, "\"oneOne, twoTwo\"")]
    [InlineData(true, true, TestEnum.Four, "\"foo\"")]
    // Disallow integer
    [InlineData(false, false, TestEnum.None, "\"None\"")]
    [InlineData(false, false, TestEnum.OneOne, "\"One_one\"")]
    [InlineData(false, false, TestEnum.TwoTwo, "\"TwoTwo\"")]
    [InlineData(false, false, TestEnum.OneOne | TestEnum.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(false, false, TestEnum.Four, "\"foo\"")]
    public void CanSerializeJson(bool allowInteger, bool useCamelCase, TestEnum @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption(allowInteger, useCamelCase)).Should().Be(expected);

    [Theory]
    // Disallow integer
    [InlineData(false, false, (TestEnum)(-1))]
    public void CannotSerializeJson(bool allowInteger, bool useCamelCase, TestEnum @enum)
    {
        var action = () => JsonSerializer.Serialize(@enum, CreateOption(allowInteger, useCamelCase));
        action.Should().Throw<JsonException>();
    }

    [Theory]
    // Default
    [InlineData(true, false, "-1", (TestEnum)(-1))]
    [InlineData(true, false, "\"None\"", TestEnum.None)]
    [InlineData(true, false, "\"One_one\"", TestEnum.OneOne)]
    [InlineData(true, false, "\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData(true, false, "\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData(true, false, "\"4\"", TestEnum.Four)]
    // Use JsonNamingPolicy.CamelCase
    [InlineData(true, true, "-1", (TestEnum)(-1))]
    [InlineData(true, true, "\"none\"", TestEnum.None)]
    [InlineData(true, true, "\"One_one\"", TestEnum.OneOne)]
    [InlineData(true, true, "\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData(true, true, "\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData(true, true, "\"4\"", TestEnum.Four)]
    // Disallow integer
    [InlineData(false, false, "\"None\"", TestEnum.None)]
    [InlineData(false, false, "\"One_one\"", TestEnum.OneOne)]
    [InlineData(false, false, "\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData(false, false, "\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData(false, false, "\"4\"", TestEnum.Four)]
    public void CanDeserializeJson(bool allowInteger, bool useCamelCase, string json, TestEnum expected)
        => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(allowInteger, useCamelCase)).Should().Be(expected);

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
        var action = () => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(allowInteger, useCamelCase));
        action.Should().Throw<JsonException>();
    }

    /// <summary>For <see langword="int"/> based testing</summary>
    private enum TestEnumInt32 : int { One = 1 }
    /// <summary>For <see langword="uint"/> based testing</summary>
    private enum TestEnumUInt32 : uint { One = 1 }
    /// <summary>For <see langword="ulong"/> based testing</summary>
    private enum TestEnumUInt64 : ulong { One = 1 }
    /// <summary>For <see langword="long"/> based testing</summary>
    private enum TestEnumInt64 : long { One = 1 }
    /// <summary>For <see langword="sbyte"/> based testing</summary>
    private enum TestEnumSByte : sbyte { One = 1 }
    /// <summary>For <see langword="byte"/> based testing</summary>
    private enum TestEnumByte : byte { One = 1 }
    /// <summary>For <see langword="short"/> based testing</summary>
    private enum TestEnumInt16 : short { One = 1 }
    /// <summary>For <see langword="ushort"/> based testing</summary>
    private enum TestEnumUInt16 : ushort { One = 1 }

    [Fact]
    public void CanSerializeJson_Type()
    {
        var option = CreateOption(true, false);
        JsonSerializer.Serialize(TestEnumInt32.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumUInt32.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumUInt64.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumInt64.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumSByte.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumByte.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumInt16.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize(TestEnumUInt16.One, option).Should().Be("\"One\"");
        JsonSerializer.Serialize((TestEnumInt32)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumUInt32)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumUInt64)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumInt64)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumSByte)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumByte)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumInt16)2, option).Should().Be("2");
        JsonSerializer.Serialize((TestEnumUInt16)2, option).Should().Be("2");
    }

    [Fact]
    public void CanDeserializeJson_Type()
    {
        var option = CreateOption(true, false);
        JsonSerializer.Deserialize<TestEnumInt32>("1", option).Should().Be(TestEnumInt32.One);
        JsonSerializer.Deserialize<TestEnumUInt32>("1", option).Should().Be(TestEnumUInt32.One);
        JsonSerializer.Deserialize<TestEnumUInt64>("1", option).Should().Be(TestEnumUInt64.One);
        JsonSerializer.Deserialize<TestEnumInt64>("1", option).Should().Be(TestEnumInt64.One);
        JsonSerializer.Deserialize<TestEnumSByte>("1", option).Should().Be(TestEnumSByte.One);
        JsonSerializer.Deserialize<TestEnumByte>("1", option).Should().Be(TestEnumByte.One);
        JsonSerializer.Deserialize<TestEnumInt16>("1", option).Should().Be(TestEnumInt16.One);
        JsonSerializer.Deserialize<TestEnumUInt16>("1", option).Should().Be(TestEnumUInt16.One);
    }
}
