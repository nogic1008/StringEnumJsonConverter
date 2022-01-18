namespace Nogic.JsonConverters;

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

/// <inheritdoc cref="EnumStringConverter" />
/// <typeparam name="TEnum"><see langword="enum"/> type</typeparam>
public class EnumStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly TypeCode _enumTypeCode = Type.GetTypeCode(typeof(TEnum));
    private readonly Dictionary<TEnum, string> _enumToString = new();
    private readonly Dictionary<string, TEnum> _stringToEnum = new();

    /// <summary>
    /// <inheritdoc cref="EnumStringConverter(bool)" path="/param[@name='allowIntegerValues']"/>
    /// </summary>
    private readonly bool _allowIntegerValues;

    /// <inheritdoc cref="EnumStringConverter(bool)" />
    public EnumStringConverter(bool allowIntegerValues = true)
    {
        _allowIntegerValues = allowIntegerValues;
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
                _stringToEnum.Add(attr.Value!, value);
            }
            else
            {
                _enumToString.Add(value, value.ToString());
            }
        }
    }

    public override bool CanConvert(Type type) => type.IsEnum;

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (!_allowIntegerValues)
                throw new JsonException();
            return _enumTypeCode switch
            {
                TypeCode.Int32 => AsTEnum(reader.TryGetInt32(out int int32), ref int32),
                TypeCode.UInt32 => AsTEnum(reader.TryGetUInt32(out uint uint32), ref uint32),
                TypeCode.UInt64 => AsTEnum(reader.TryGetUInt64(out ulong uint64), ref uint64),
                TypeCode.Int64 => AsTEnum(reader.TryGetInt64(out long int64), ref int64),
                TypeCode.SByte => AsTEnum(reader.TryGetSByte(out sbyte byte8), ref byte8),
                TypeCode.Byte => AsTEnum(reader.TryGetByte(out byte ubyte8), ref ubyte8),
                TypeCode.Int16 => AsTEnum(reader.TryGetInt16(out short int16), ref int16),
                TypeCode.UInt16 => AsTEnum(reader.TryGetUInt16(out ushort uint16), ref uint16),
                _ => throw new JsonException(),
            };
        }
        return _stringToEnum.TryGetValue(reader.GetString()!, out var enumValue) ? enumValue : throw new JsonException();

        static TEnum AsTEnum<T>(bool success, ref T value)
            => success ? Unsafe.As<T, TEnum>(ref value) : throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteStringValue(_enumToString.TryGetValue(value, out string? json) ? json : throw new JsonException());
}
