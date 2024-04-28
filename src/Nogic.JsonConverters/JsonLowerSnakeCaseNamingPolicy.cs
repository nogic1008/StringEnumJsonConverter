namespace Nogic.JsonConverters;

/// <summary>Naming policy for lower_snake_casing.</summary>
#if NET8_0_OR_GREATER
[Obsolete($"Use built-in {nameof(JsonNamingPolicy)}.{nameof(SnakeCaseLower)}.")]
#endif
public sealed class JsonLowerSnakeCaseNamingPolicy() : JsonSeparatorNamingPolicy(true, '_');