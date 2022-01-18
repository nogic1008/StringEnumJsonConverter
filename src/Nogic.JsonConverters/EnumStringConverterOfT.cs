namespace Nogic.JsonConverters;

using System.Runtime.Serialization;

public class EnumStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumToString = new();
    private readonly Dictionary<string, TEnum> _stringToEnum = new();

    public EnumStringConverter()
    {
        var type = typeof(TEnum);
#if NET5_0_OR_GREATER
        foreach (var value in Enum.GetValues<TEnum>())
#else
        foreach (TEnum value in Enum.GetValues(type))
#endif
        {
            var enumMember = type.GetMember(value.ToString())[0];
            var attr = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            _stringToEnum.Add(value.ToString(), value);

            if (attr?.IsValueSetExplicitly is true)
            {
                _enumToString.Add(value, attr.Value!);
                _stringToEnum.Add(attr.Value, value);
            }
            else
            {
                _enumToString.Add(value, value.ToString());
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => _stringToEnum.TryGetValue(reader.GetString()!, out var enumValue) ? enumValue : throw new JsonException();

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteStringValue(_enumToString.TryGetValue(value, out string json) ? json : throw new JsonException());
}
