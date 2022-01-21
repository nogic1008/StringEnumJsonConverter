using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Nogic.JsonConverters;

/// <inheritdoc cref="EnumStringConverter" />
/// <typeparam name="TEnum"><see langword="enum"/> type</typeparam>
public class EnumStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly TypeCode _enumTypeCode = Type.GetTypeCode(typeof(TEnum));
    private static readonly string? _negativeSign = ((int)_enumTypeCode % 2) == 0 ? null : NumberFormatInfo.CurrentInfo.NegativeSign;

    /// <summary>Mapping <typeparamref name="TEnum"/> to UTF-8 string.</summary>
    private readonly ConcurrentDictionary<ulong, JsonEncodedText> _nameCache = new();

    /// <summary>Mapping <see langword="string"/> to <typeparamref name="TEnum"/>.</summary>
    private readonly ConcurrentDictionary<string, TEnum> _valueCache = new();

    /// <summary>
    /// This is used to prevent flooding the cache due to exponential bitwise combinations of flags.
    /// Since multiple threads can add to the cache, a few more values might be added.
    /// </summary>
    private const int NameCacheSizeSoftLimit = 64;

    /// <summary>
    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" path="/param[@name='allowIntegerValues']"/>
    /// </summary>
    private readonly bool _allowIntegerValues;

    /// <summary>
    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" path="/param[@name='namingPolicy']"/>
    /// </summary>
    private readonly JsonNamingPolicy? _namingPolicy;

    /// <inheritdoc cref="EnumStringConverter(bool, JsonNamingPolicy?)" />
    public EnumStringConverter(bool allowIntegerValues, JsonNamingPolicy? namingPolicy, JsonSerializerOptions serializerOptions)
    {
        _allowIntegerValues = allowIntegerValues;
        _namingPolicy = namingPolicy;

        var typeToConvert = typeof(TEnum);
        var values = GetEnumValues();
        var encoder = serializerOptions.Encoder;

        foreach (var item in values)
        {
            ulong key = ConvertToUInt64(item);
            var attr = GetAttribute(item);
            string value = attr?.Value ?? ConvertName(item.ToString());
            if (!TryAddNameCache(key, value, serializerOptions))
                break;
            if (attr?.IsValueSetExplicitly == true)
                _valueCache.TryAdd(value, item);
        }

        TEnum[] GetEnumValues() =>
#if NET5_0_OR_GREATER
            Enum.GetValues<TEnum>();
#else
            (TEnum[])Enum.GetValues(typeToConvert);
#endif

        EnumMemberAttribute? GetAttribute(TEnum value)
            => typeToConvert.GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TEnum);

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && _allowIntegerValues)
        {
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
                _ => throw new JsonException(), // This is dead path because TEnum is only based on above type.
            };
        }

        string enumString = reader.GetString()!;
        if (!_valueCache.TryGetValue(enumString, out var value) && !Enum.TryParse(enumString, ignoreCase: true, out value))
            throw new JsonException();

        return value;

        static TEnum AsTEnum<T>(bool success, ref T value)
            => success ? Unsafe.As<T, TEnum>(ref value) : throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        ulong key = ConvertToUInt64(value);

        if (_nameCache.TryGetValue(key, out var formatted))
        {
            writer.WriteStringValue(formatted);
            return;
        }

        string original = value.ToString();
        if (IsValidIdentifier(original))
        {
            string converted = ConvertName(original);
            TryAddNameCache(key, converted, options);
            writer.WriteStringValue(converted);
            return;
        }

        if (_allowIntegerValues)
        {
            switch (_enumTypeCode)
            {
                case TypeCode.Int32:
                    writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
                    break;
                case TypeCode.UInt32:
                    writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
                    break;
                case TypeCode.UInt64:
                    writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
                    break;
                case TypeCode.Int64:
                    writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
                    break;
                case TypeCode.Int16:
                    writer.WriteNumberValue(Unsafe.As<TEnum, short>(ref value));
                    break;
                case TypeCode.UInt16:
                    writer.WriteNumberValue(Unsafe.As<TEnum, ushort>(ref value));
                    break;
                case TypeCode.Byte:
                    writer.WriteNumberValue(Unsafe.As<TEnum, byte>(ref value));
                    break;
                case TypeCode.SByte:
                    writer.WriteNumberValue(Unsafe.As<TEnum, sbyte>(ref value));
                    break;
                default:
                    throw new JsonException(); // This is dead path because TEnum is only based on above type.
            }
            return;
        }

        throw new JsonException();

        static bool IsValidIdentifier(string value) =>
            value[0] >= 'A' && (_negativeSign is null || !value.StartsWith(_negativeSign, StringComparison.Ordinal));
    }

    private static ulong ConvertToUInt64(object value)
        => _enumTypeCode switch
        {
            TypeCode.Int32 => (ulong)(int)value,
            TypeCode.UInt32 => (uint)value,
            TypeCode.UInt64 => (ulong)value,
            TypeCode.Int64 => (ulong)(long)value,
            TypeCode.SByte => (ulong)(sbyte)value,
            TypeCode.Byte => (byte)value,
            TypeCode.Int16 => (ulong)(short)value,
            TypeCode.UInt16 => (ushort)value,
            _ => throw new InvalidOperationException(), // This is dead path because TEnum is only based on above type.
        };

    private bool TryAddNameCache(ulong key, string value, JsonSerializerOptions options)
    {
        if (_nameCache.Count >= NameCacheSizeSoftLimit)
            return false;
        var encoder = options.Encoder;
        _nameCache.TryAdd(key, JsonEncodedText.Encode(value, encoder));
        return true;
    }

    private string ConvertName(string value)
    {
        if (_namingPolicy is null)
            return value;

        const string ValueSeparator = ", ";
        if (!value.Contains(ValueSeparator, StringComparison.Ordinal))
            return _namingPolicy.ConvertName(value);

        string[] enumValues = value.Split(new string[] { ValueSeparator }, StringSplitOptions.None);
        for (int i = 0; i < enumValues.Length; i++)
            enumValues[i] = _namingPolicy.ConvertName(enumValues[i]);
        return string.Join(ValueSeparator, enumValues);
    }
}
