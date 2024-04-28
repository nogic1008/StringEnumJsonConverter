namespace Nogic.JsonConverters;

/// <summary>Naming policy for lower_snake_casing.</summary>
#if NET8_0_OR_GREATER
[Obsolete($"Use built-in {nameof(JsonNamingPolicy)}.{nameof(SnakeCaseLower)}.")]
#endif
public sealed class JsonLowerSnakeCaseNamingPolicy : JsonSeparatorNamingPolicy
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonLowerSnakeCaseNamingPolicy"/>
    /// </summary>
    public JsonLowerSnakeCaseNamingPolicy() : base(true, '_') { }
}
