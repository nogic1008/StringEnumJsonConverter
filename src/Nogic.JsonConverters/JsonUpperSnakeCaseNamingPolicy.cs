namespace Nogic.JsonConverters;

/// <summary>Naming policy for UPPER_SNAKE_CASING.</summary>
#if NET8_0_OR_GREATER
[Obsolete($"Use built-in {nameof(JsonNamingPolicy)}.{nameof(SnakeCaseUpper)}.")]
#endif
public sealed class JsonUpperSnakeCaseNamingPolicy() : JsonSeparatorNamingPolicy(false, '_');
