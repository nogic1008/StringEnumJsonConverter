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

    private JsonSerializerOptions CreateOption(bool allowIntegerValues = true, JsonNamingPolicy? namingPolicy = null)
        => new()
        {
            Converters = { new EnumStringConverter(allowIntegerValues, namingPolicy) }
        };

    [Theory]
    [InlineData((TestEnum)(-1), "-1")]
    [InlineData(TestEnum.None, "\"None\"")]
    [InlineData(TestEnum.OneOne, "\"One_one\"")]
    [InlineData(TestEnum.TwoTwo, "\"TwoTwo\"")]
    [InlineData(TestEnum.OneOne | TestEnum.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(TestEnum.Four, "\"foo\"")]
    public void CanSerializeJson_Default(TestEnum @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption()).Should().Be(expected);

    [Theory]
    [InlineData("-1", (TestEnum)(-1))]
    [InlineData("\"None\"", TestEnum.None)]
    [InlineData("\"One_one\"", TestEnum.OneOne)]
    [InlineData("\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData("\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData("\"4\"", TestEnum.Four)]
    public void CanDeserializeJson_Default(string json, TestEnum expected)
        => JsonSerializer.Deserialize<TestEnum>(json, CreateOption()).Should().Be(expected);

    [Theory]
    [InlineData("\"\"")]
    [InlineData("OneOne")]
    [InlineData("twotwo")]
    [InlineData("\"One_one, TwoTwo\"")]
    public void CannotDeserializeJson_Default(string json)
    {
        var action = () => JsonSerializer.Deserialize<TestEnum>(json, CreateOption());
        action.Should().Throw<JsonException>();
    }

    [Theory]
    [InlineData((TestEnum)(-1), "-1")]
    [InlineData(TestEnum.None, "\"none\"")]
    [InlineData(TestEnum.OneOne, "\"One_one\"")]
    [InlineData(TestEnum.TwoTwo, "\"twoTwo\"")]
    [InlineData(TestEnum.OneOne | TestEnum.TwoTwo, "\"oneOne, twoTwo\"")]
    [InlineData(TestEnum.Four, "\"foo\"")]
    public void CanSerializeJson_CamelCase(TestEnum @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption(namingPolicy: JsonNamingPolicy.CamelCase))
            .Should().Be(expected);

    [Theory]
    [InlineData("-1", (TestEnum)(-1))]
    [InlineData("\"none\"", TestEnum.None)]
    [InlineData("\"One_one\"", TestEnum.OneOne)]
    [InlineData("\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData("\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData("\"4\"", TestEnum.Four)]
    public void CanDeserializeJson_CamelCase(string json, TestEnum expected)
        => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(namingPolicy: JsonNamingPolicy.CamelCase))
            .Should().Be(expected);

    [Theory]
    [InlineData("\"\"")]
    [InlineData("OneOne")]
    [InlineData("twotwo")]
    [InlineData("\"One_one, TwoTwo\"")]
    public void CannotDeserializeJson_CamelCase(string json)
    {
        var action = () => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(namingPolicy: JsonNamingPolicy.CamelCase));
        action.Should().Throw<JsonException>();
    }

    [Theory]
    [InlineData(TestEnum.None, "\"None\"")]
    [InlineData(TestEnum.OneOne, "\"One_one\"")]
    [InlineData(TestEnum.TwoTwo, "\"TwoTwo\"")]
    [InlineData(TestEnum.OneOne | TestEnum.TwoTwo, "\"OneOne, TwoTwo\"")]
    [InlineData(TestEnum.Four, "\"foo\"")]
    public void CanSerializeJson_DisallowInteger(TestEnum @enum, string expected)
        => JsonSerializer.Serialize(@enum, CreateOption(allowIntegerValues: false))
            .Should().Be(expected);

    [Fact]
    public void CannotSerializeJson_DisallowInteger()
    {
        var action = () => JsonSerializer.Serialize((TestEnum)(-1), CreateOption(allowIntegerValues: false));
        action.Should().Throw<JsonException>();
    }

    [Theory]
    [InlineData("\"None\"", TestEnum.None)]
    [InlineData("\"One_one\"", TestEnum.OneOne)]
    [InlineData("\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData("\"OneOne, TwoTwo\"", TestEnum.OneOne | TestEnum.TwoTwo)]
    [InlineData("\"4\"", TestEnum.Four)]
    public void CanDeserializeJson_DisallowInteger(string json, TestEnum expected)
        => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(allowIntegerValues: false))
            .Should().Be(expected);

    [Theory]
    [InlineData("\"\"")]
    [InlineData("-1")]
    [InlineData("OneOne")]
    [InlineData("twotwo")]
    [InlineData("\"One_one, TwoTwo\"")]
    public void CannotDeserializeJson_DisallowInteger(string json)
    {
        var action = () => JsonSerializer.Deserialize<TestEnum>(json, CreateOption(allowIntegerValues: false));
        action.Should().Throw<JsonException>();
    }
}
