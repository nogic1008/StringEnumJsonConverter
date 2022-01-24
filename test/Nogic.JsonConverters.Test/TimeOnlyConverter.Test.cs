using System.Globalization;

namespace Nogic.JsonConverters.Test;

/// <summary>Unit test of <see cref="TimeOnlyConverter"/></summary>
public class TimeOnlyConverterTest
{
    /// <summary>
    /// Create <see cref="JsonSerializerOptions"/> that contains <see cref="TimeOnlyConverter"/>.
    /// </summary>
    /// <param name="format">
    /// <inheritdoc cref="TimeOnlyConverter(string, IFormatProvider?)" path="/param[@name='serializationFormat']"/>
    /// </param>
    private static JsonSerializerOptions CreateOption(string format)
        => new() { Converters = { new TimeOnlyConverter(format, CultureInfo.InvariantCulture) } };

    /// <summary>
    /// <see cref="TimeOnlyConverter.Read"/> returns expected <see cref="TimeOnly"/> object.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='format']"/>
    /// <param name="json">JSON string</param>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='hour']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='minute']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='second']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='millisecond']"/>
    [Theory]
    [InlineData("HH:mm:ss.fff", "\"22:30:30.300\"", 22, 30, 30, 300)]
    [InlineData("hh:mm:ss tt", "\"10:00:00 PM\"", 22, 0, 0, 0)]
    [InlineData("HHmm", "\"2200\"", 22, 0, 0, 0)]
    public void CanDeserializeJson(string format, string json, int hour, int minute, int second, int millisecond)
        => JsonSerializer.Deserialize<TimeOnly>(json, CreateOption(format)).Should().Be(new(hour, minute, second, millisecond));


    /// <summary>
    /// <see cref="DateOnlyConverter.Read"/> throws error.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='format']"/>
    /// <param name="json">JSON string</param>
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

    /// <summary>
    /// <see cref="DateOnlyConverter.Write"/> writes <paramref name="expected"/>.
    /// </summary>
    /// <inheritdoc cref="CreateOption" path="/param[@name='format']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='hour']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='minute']"/>
    /// <inheritdoc cref="TimeOnly(int, int, int, int)" path="/param[@name='second']"/>
    /// <param name="expected">Expected JSON string</param>
    [Theory]
    [InlineData("HH:mm:ss.fff", 0, 0, 0, "\"00:00:00.000\"")]
    [InlineData("HHmm", 22, 0, 0, "\"2200\"")]
    [InlineData("hh:mm:ss tt", 23, 59, 59, "\"11:59:59 PM\"")]
    public void CanSerializeJson(string format, int hour, int minute, int second, string expected)
        => JsonSerializer.Serialize(new TimeOnly(hour, minute, second, 0), CreateOption(format)).Should().Be(expected);
}
