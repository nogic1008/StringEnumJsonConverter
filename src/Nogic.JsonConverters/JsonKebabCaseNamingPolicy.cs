namespace Nogic.JsonConverters;

/// <summary>Naming policy for kebab-casing.</summary>
public sealed class JsonKebabCaseNamingPolicy : JsonNamingPolicyBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonKebabCaseNamingPolicy"/>
    /// </summary>
    public JsonKebabCaseNamingPolicy() : base('-') { }

    /// <inheritdoc/>
    protected override char ConvertForWrite(bool isTopOfWord, char c) => char.ToLowerInvariant(c);
}
