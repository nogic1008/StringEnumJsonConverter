namespace Nogic.JsonConverters;

public abstract class JsonLowerCaseNamingPolicy : JsonNamingPolicy
{
    protected readonly string _separator;
    protected JsonLowerCaseNamingPolicy(string separator) => _separator = separator;
    public override string ConvertName(string name)
        => string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? _separator + x.ToString() : x.ToString())).ToLower();
}

public class JsonSnakeCaseNamingPolicy : JsonLowerCaseNamingPolicy
{
    public JsonSnakeCaseNamingPolicy() : base("_") { }
}

public sealed class JsonKebabCaseNamingPolicy : JsonLowerCaseNamingPolicy
{
    public JsonKebabCaseNamingPolicy() : base("-") { }
}
