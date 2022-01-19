namespace Nogic.JsonConverters.Test;

public class JsonLowerCaseNamingPolicyTest
{
    [Theory]
    [InlineData("", "")]
    [InlineData("PascalCase", "pascal_case")]
    [InlineData("camelCase", "camel_case")]
    [InlineData("snake_case", "snake_case")]
    [InlineData("kebab-case", "kebab-case")]
    [InlineData("Firstupper", "firstupper")]
    public void JsonSnakeCaseNamingPolicy_ConvertName(string name, string expected)
        => new JsonSnakeCaseNamingPolicy().ConvertName(name).Should().Be(expected);

    [Theory]
    [InlineData("", "")]
    [InlineData("PascalCase", "pascal-case")]
    [InlineData("camelCase", "camel-case")]
    [InlineData("snake_case", "snake_case")]
    [InlineData("kebab-case", "kebab-case")]
    [InlineData("Firstupper", "firstupper")]
    public void JsonKebabCaseNamingPolicy_ConvertName(string name, string expected)
        => new JsonKebabCaseNamingPolicy().ConvertName(name).Should().Be(expected);
}
