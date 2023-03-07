namespace Nogic.JsonConverters.Test;

/// <summary>Unit test for <see cref="JsonNamingPolicyBase"/></summary>
public sealed class JsonLowerCaseNamingPolicyTest
{
    /// <summary>Unit test for <see cref="JsonLowerSnakeCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("PascalCase", "pascal_case")]
    [InlineData("camelCase", "camel_case")]
    [InlineData("snake_case", "snake_case")]
    [InlineData("kebab-case", "kebab_case")]
    [InlineData("word  word", "word_word")]
    [InlineData("UPPER", "upper")]
    [InlineData("lower", "lower")]
    [InlineData("camelUPPER", "camel_upper")]
    [InlineData("WithNumber123", "with_number_123")]
    public void JsonLowerSnakeCaseNamingPolicy_ConvertName(string name, string expected)
        => new JsonLowerSnakeCaseNamingPolicy().ConvertName(name).Should().Be(expected);

    /// <summary>Unit test for <see cref="JsonUpperSnakeCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("PascalCase", "PASCAL_CASE")]
    [InlineData("camelCase", "CAMEL_CASE")]
    [InlineData("snake_case", "SNAKE_CASE")]
    [InlineData("kebab-case", "KEBAB_CASE")]
    [InlineData("word  word", "WORD_WORD")]
    [InlineData("UPPER", "UPPER")]
    [InlineData("lower", "LOWER")]
    [InlineData("camelUPPER", "CAMEL_UPPER")]
    [InlineData("WithNumber123", "WITH_NUMBER_123")]
    public void JsonUpperSnakeCaseNamingPolicy_ConvertName(string name, string expected)
        => new JsonUpperSnakeCaseNamingPolicy().ConvertName(name).Should().Be(expected);

    /// <summary>Unit test for <see cref="JsonKebabCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("PascalCase", "pascal-case")]
    [InlineData("camelCase", "camel-case")]
    [InlineData("snake_case", "snake-case")]
    [InlineData("kebab-case", "kebab-case")]
    [InlineData("word  word", "word-word")]
    [InlineData("UPPER", "upper")]
    [InlineData("lower", "lower")]
    [InlineData("camelUPPER", "camel-upper")]
    [InlineData("WithNumber123", "with-number-123")]
    public void JsonKebabCaseNamingPolicy_ConvertName(string name, string expected)
        => new JsonKebabCaseNamingPolicy().ConvertName(name).Should().Be(expected);
}
