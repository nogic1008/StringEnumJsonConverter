namespace Nogic.JsonConverters.Test;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>Unit test of <see cref="EnumStringConverter"/></summary>
public sealed class EnumStringConverterTest
{
    [JsonConverter(typeof(EnumStringConverter<TestEnum>))]
    public enum TestEnum
    {
        None = 0,
        [EnumMember(Value = "one_one")] OneOne = 1,
        [EnumMember] TwoTwo = 2,
        [EnumMember(Value = "foo")] Four = 4,
    }

    [Theory]
    [InlineData(TestEnum.None, "\"None\"")]
    [InlineData(TestEnum.OneOne, "\"one_one\"")]
    [InlineData(TestEnum.TwoTwo, "\"TwoTwo\"")]
    [InlineData(TestEnum.Four, "\"foo\"")]
    public void CanSerializeJson(TestEnum @enum, string expected)
        => JsonSerializer.Serialize(@enum).Should().Be(expected);

    [Theory]
    [InlineData(3)]
    [InlineData(100)]
    public void CannotSerializeJson(int @enum)
    {
        var action = () => JsonSerializer.Serialize((TestEnum)@enum);
        action.Should().Throw<JsonException>();
    }

    [Theory]
    [InlineData("\"None\"", TestEnum.None)]
    [InlineData("\"one_one\"", TestEnum.OneOne)]
    [InlineData("\"TwoTwo\"", TestEnum.TwoTwo)]
    [InlineData("\"foo\"", TestEnum.Four)]
    public void CanDeserializeJson(string json, TestEnum expected)
        => JsonSerializer.Deserialize<TestEnum>(json).Should().Be(expected);

    [Theory]
    [InlineData("\"\"")]
    [InlineData("1")]
    [InlineData("OneOne")]
    [InlineData("twotwo")]
    public void CannotDeserializeJson(string json)
    {
        var action = () => JsonSerializer.Deserialize<TestEnum>(json);
        action.Should().Throw<JsonException>();
    }
}
