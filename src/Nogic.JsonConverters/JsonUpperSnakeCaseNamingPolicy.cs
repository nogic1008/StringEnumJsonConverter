namespace Nogic.JsonConverters;

/// <summary>Naming policy for UPPER_SNAKE_CASING.</summary>
public sealed class JsonUpperSnakeCaseNamingPolicy : JsonNamingPolicyBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonUpperSnakeCaseNamingPolicy"/>
    /// </summary>
    public JsonUpperSnakeCaseNamingPolicy() : base('_') { }

    /// <inheritdoc/>
    protected override char ConvertForWrite(bool isTopOfWord, char c) => char.ToUpperInvariant(c);
}
