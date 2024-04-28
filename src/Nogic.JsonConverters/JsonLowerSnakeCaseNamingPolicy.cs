namespace Nogic.JsonConverters;

/// <summary>Naming policy for lower_snake_casing.</summary>
#if NET8_0_OR_GREATER
[Obsolete($"Use built-in {nameof(JsonNamingPolicy)}.{nameof(SnakeCaseLower)}.")]
#endif
public sealed class JsonLowerSnakeCaseNamingPolicy : JsonNamingPolicyBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonLowerSnakeCaseNamingPolicy"/>
    /// </summary>
    public JsonLowerSnakeCaseNamingPolicy() : base('_') { }

    /// <inheritdoc/>
    protected override char ConvertForWrite(bool isTopOfWord, char c) => char.ToLowerInvariant(c);
}
