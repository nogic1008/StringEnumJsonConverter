namespace Nogic.JsonConverters.Test;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>Unit test of <see cref="EnumStringConverter"/></summary>
public sealed class EnumStringConverterTest
{
    [Flags]
    public enum TestEnum
    {
        None = 0,
        [EnumMember(Value = "One_one")] OneOne = 1,
        [EnumMember] TwoTwo = 2,
        [EnumMember(Value = "foo")] Four = 4,
    }

    private JsonSerializerOptions CreateOption(bool allowInteger, bool useCamelCase)
        => new()
        {
            Converters = { new EnumStringConverter(allowInteger, useCamelCase ? JsonNamingPolicy.CamelCase : null) }
        };

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

}
