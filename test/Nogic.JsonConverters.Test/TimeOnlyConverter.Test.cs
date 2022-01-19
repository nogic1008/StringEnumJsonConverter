#if NET6_0_OR_GREATER
namespace Nogic.JsonConverters.Test;

using System.Globalization;

/// <summary>Unit test of <see cref="TimeOnlyConverter"/></summary>
public class TimeOnlyConverterTest
{
    private static JsonSerializerOptions CreateOption(string format)
        => new() { Converters = { new TimeOnlyConverter(format, CultureInfo.InvariantCulture) } };

    [Theory]
    [InlineData("HH:mm:ss.fff", "\"22:30:30.300\"", 22, 30, 30, 300)]
    [InlineData("hh:mm:ss tt", "\"10:00:00 PM\"", 22, 0, 0, 0)]
    [InlineData("HHmm", "\"2200\"", 22, 0, 0, 0)]
    public void CanDeserializeJson(string format, string json, int hour, int minute, int second, int millisecond)
        => JsonSerializer.Deserialize<TimeOnly>(json, CreateOption(format)).Should().Be(new(hour, minute, second, millisecond));

    [Theory]
    [InlineData("HH:mm:ss.fff", "\"10:00:00 PM\"")]
    [InlineData("hh:mm:ss tt", "\"22:30:30\"")]
    [InlineData("HHmm", "\"22:00\"")]
    public void CannotDeserializeJson(string format, string json)
    {
        // Arrange - Act
        var action = () => JsonSerializer.Deserialize<TimeOnly>(json, CreateOption(format));

        // Assert
        action.Should().Throw<FormatException>();
    }

    private static IEnumerable<object[]> CanSerializeJsonData() => new[]
    {
        new object[] { "HH:mm:ss.fff", TimeOnly.MinValue, "\"00:00:00.000\"" },
        new object[] { "HHmm", new TimeOnly(22, 0, 0), "\"2200\"" },
        new object[] { "hh:mm:ss tt", TimeOnly.MaxValue, "\"11:59:59 PM\"" },
    };
    [Theory]
    [MemberData(nameof(CanSerializeJsonData))]
    public void CanSerializeJson(string format, TimeOnly time, string expected)
        => JsonSerializer.Serialize(time, CreateOption(format)).Should().Be(expected);
}
#endif
