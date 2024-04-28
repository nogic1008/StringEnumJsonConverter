namespace Nogic.JsonConverters.Test;

/// <summary>Unit test for <see cref="JsonNamingPolicyBase"/></summary>
public sealed class JsonLowerCaseNamingPolicyTest
{
    /// <summary>Unit test for <see cref="JsonLowerSnakeCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [TestMethod]
    [DataRow("", "")]
    [DataRow("PascalCase", "pascal_case")]
    [DataRow("camelCase", "camel_case")]
    [DataRow("snake_case", "snake_case")]
    [DataRow("kebab-case", "kebab_case")]
    [DataRow("word  word", "word_word")]
    [DataRow("UPPER", "upper")]
    [DataRow("lower", "lower")]
    [DataRow("camelUPPER", "camel_upper")]
    [DataRow("WithNumber123", "with_number123")]
    public void JsonLowerSnakeCaseNamingPolicy_ConvertName(string name, string expected)
#if NET8_0_OR_GREATER
#pragma warning disable CS0618
#endif
        => new JsonLowerSnakeCaseNamingPolicy().ConvertName(name).Should().Be(expected);
#if NET8_0_OR_GREATER
#pragma warning restore CS0618
#endif

    /// <summary>Unit test for <see cref="JsonUpperSnakeCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [TestMethod]
    [DataRow("", "")]
    [DataRow("PascalCase", "PASCAL_CASE")]
    [DataRow("camelCase", "CAMEL_CASE")]
    [DataRow("snake_case", "SNAKE_CASE")]
    [DataRow("kebab-case", "KEBAB_CASE")]
    [DataRow("word  word", "WORD_WORD")]
    [DataRow("UPPER", "UPPER")]
    [DataRow("lower", "LOWER")]
    [DataRow("camelUPPER", "CAMEL_UPPER")]
    [DataRow("WithNumber123", "WITH_NUMBER123")]
    public void JsonUpperSnakeCaseNamingPolicy_ConvertName(string name, string expected)
#if NET8_0_OR_GREATER
#pragma warning disable CS0618
#endif
        => new JsonUpperSnakeCaseNamingPolicy().ConvertName(name).Should().Be(expected);
#if NET8_0_OR_GREATER
#pragma warning restore CS0618
#endif

    /// <summary>Unit test for <see cref="JsonKebabCaseNamingPolicy"/></summary>
    /// <param name="name">Original name</param>
    /// <param name="expected">Converted name</param>
    [TestMethod]
    [DataRow("", "")]
    [DataRow("PascalCase", "pascal-case")]
    [DataRow("camelCase", "camel-case")]
    [DataRow("snake_case", "snake-case")]
    [DataRow("kebab-case", "kebab-case")]
    [DataRow("word  word", "word-word")]
    [DataRow("UPPER", "upper")]
    [DataRow("lower", "lower")]
    [DataRow("camelUPPER", "camel-upper")]
    [DataRow("WithNumber123", "with-number123")]
    public void JsonKebabCaseNamingPolicy_ConvertName(string name, string expected)
#if NET8_0_OR_GREATER
#pragma warning disable CS0618
#endif
        => new JsonKebabCaseNamingPolicy().ConvertName(name).Should().Be(expected);
#if NET8_0_OR_GREATER
#pragma warning restore CS0618
#endif
}
