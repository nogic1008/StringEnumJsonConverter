// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/v8.0.0-preview.1.23110.8/src/libraries/System.Text.Json/Common/JsonSeparatorNamingPolicy.cs

using System.Buffers;
using System.ComponentModel;
using System.Globalization;

namespace Nogic.JsonConverters;

/// <summary>Helper for <see cref="JsonNamingPolicy"/></summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class JsonSeparatorNamingPolicy : JsonNamingPolicy
{
    // https://github.com/dotnet/runtime/blob/v8.0.0-preview.1.23110.8/src/libraries/System.Text.Json/Common/JsonConstants.cs
    private const int StackallocByteThreshold = 256;
    private const int StackallocCharThreshold = StackallocByteThreshold / 2;

    private readonly bool _lowercase;
    private readonly char _separator;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonSeparatorNamingPolicy"/>.
    /// </summary>
    /// <param name="lowercase">Lowercased or not</param>
    /// <param name="separator">Char separator</param>
    protected JsonSeparatorNamingPolicy(bool lowercase, char separator) =>
        (_lowercase, _separator) = (lowercase, separator);

    /// <inheritdoc/>
    public sealed override string ConvertName(string name)
    {
        // Rented buffer 20% longer that the input.
        int rentedBufferLength = 12 * name.Length / 10;
        char[]? rentedBuffer = rentedBufferLength > StackallocCharThreshold
            ? ArrayPool<char>.Shared.Rent(rentedBufferLength)
            : null;

        int resultUsedLength = 0;
        Span<char> result = rentedBuffer ?? (stackalloc char[StackallocCharThreshold]);

        void ExpandBuffer(ref Span<char> result)
        {
            char[] newBuffer = ArrayPool<char>.Shared.Rent(result.Length * 2);

            result.CopyTo(newBuffer);

            if (rentedBuffer is not null)
            {
                result[..resultUsedLength].Clear();
                ArrayPool<char>.Shared.Return(rentedBuffer);
            }

            rentedBuffer = newBuffer;
            result = rentedBuffer;
        }

        void WriteWord(ReadOnlySpan<char> word, ref Span<char> result)
        {
            if (word.IsEmpty)
                return;

            int written;
            while (true)
            {
                int destinationOffset = resultUsedLength != 0
                    ? resultUsedLength + 1
                    : resultUsedLength;

                if (destinationOffset < result.Length)
                {
                    var destination = result[destinationOffset..];

                    written = _lowercase
                        ? word.ToLowerInvariant(destination)
                        : word.ToUpperInvariant(destination);

                    if (written > 0)
                        break;
                }

                ExpandBuffer(ref result);
            }

            if (resultUsedLength != 0)
            {
                result[resultUsedLength] = _separator;
                resultUsedLength++;
            }

            resultUsedLength += written;
        }

        int first = 0;
        var chars = name.AsSpan();
        var previousCategory = CharCategory.Boundary;

        for (int index = 0; index < chars.Length; index++)
        {
            char current = chars[index];
            var currentCategoryUnicode = char.GetUnicodeCategory(current);

            if (currentCategoryUnicode is UnicodeCategory.SpaceSeparator or
                (>= UnicodeCategory.ConnectorPunctuation and
                <= UnicodeCategory.OtherPunctuation))
            {
                WriteWord(chars[first..index], ref result);

                previousCategory = CharCategory.Boundary;
                first = index + 1;

                continue;
            }

            if (index + 1 < chars.Length)
            {
                char next = chars[index + 1];
                var currentCategory = currentCategoryUnicode switch
                {
                    UnicodeCategory.LowercaseLetter => CharCategory.Lowercase,
                    UnicodeCategory.UppercaseLetter => CharCategory.Uppercase,
                    _ => previousCategory
                };

                if ((currentCategory == CharCategory.Lowercase && char.IsUpper(next)) || next == '_')
                {
                    WriteWord(chars.Slice(first, index - first + 1), ref result);

                    previousCategory = CharCategory.Boundary;
                    first = index + 1;

                    continue;
                }

                if (previousCategory == CharCategory.Uppercase &&
                    currentCategoryUnicode == UnicodeCategory.UppercaseLetter &&
                    char.IsLower(next))
                {
                    WriteWord(chars[first..index], ref result);

                    previousCategory = CharCategory.Boundary;
                    first = index;

                    continue;
                }

                previousCategory = currentCategory;
            }
        }

        WriteWord(chars[first..], ref result);

        name = result[..resultUsedLength].ToString();

        if (rentedBuffer is not null)
        {
            result[..resultUsedLength].Clear();
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }

        return name;
    }

    private enum CharCategory
    {
        Boundary = 0,
        Lowercase = 1,
        Uppercase = 2,
    }
}
