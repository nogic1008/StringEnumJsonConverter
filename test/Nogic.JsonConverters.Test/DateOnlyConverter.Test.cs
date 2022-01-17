#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters.Test;

using System.Text.Json;
using FluentAssertions;
using Xunit;

/// <summary>Unit test of <see cref="DateOnlyConverter"/></summary>
public class DateOnlyConverterTest
{
    private static JsonSerializerOptions CreateOption(string? format)
        => new() { Converters = { new DateOnlyConverter(format) } };

    [Theory]
    [InlineData(null, "\"0001-01-01\"", 1, 1, 1)]
    [InlineData("yy/MM/dd", "\"22/01/01\"", 2022, 1, 1)]
    [InlineData("yyyyMMdd", "\"99991231\"", 9999, 12, 31)]
    public void CanDeserializeJson(string? format, string json, int year, int month, int day)
        => JsonSerializer.Deserialize<DateOnly>(json, CreateOption(format)).Should().Be(new(year, month, day));

    [Theory]
    [InlineData(null, "\"2020/01/01\"")]
    [InlineData("yyyy/MM/dd", "\"2023/10/10 0:00:00.000+09:00\"")]
    [InlineData("yy/MM/dd", "\"200101\"")]
    public void CannotDeserializeJson(string? format, string json)
    {
        // Arrange - Act
        var action = () => JsonSerializer.Deserialize<DateOnly>(json, CreateOption(format));

        // Assert
        action.Should().Throw<FormatException>();
    }

    private static IEnumerable<object?[]> CanSerializeJsonData() => new[]
    {
        new object?[] { null, DateOnly.MinValue, "\"0001-01-01\"" },
        new object?[] { "yy/MM/dd", new DateOnly(2022, 1, 1), "\"22/01/01\"" },
        new object?[] { "yyyy/MM/dd", DateOnly.MaxValue, "\"9999/12/31\"" },
    };
    [Theory]
    [MemberData(nameof(CanSerializeJsonData))]
    public void CanSerializeJson(string? format, DateOnly date, string expected)
        => JsonSerializer.Serialize(date, CreateOption(format)).Should().Be(expected);
}
#endif
