namespace Nogic.JsonConverters;

public abstract class JsonNamingPolicyBase : JsonNamingPolicy
{
    /// <summary>
    /// <inheritdoc cref="JsonNamingPolicyBase(char)" path="/param[@name='separator']"/>
    /// </summary>
    protected readonly char _separator;

    /// <summary>Constractor.</summary>
    /// <param name="separator">Word Separator</param>
    protected JsonNamingPolicyBase(char separator) => _separator = separator;

    protected virtual bool IsWordSeparator(char c)
        => char.IsUpper(c) || char.IsDigit(c) || IsSkipWrite(c);

    protected virtual bool IsSkipWrite(char c)
        => "-_".Contains(c) || char.IsWhiteSpace(c);

    protected abstract char Write(char c);

    public sealed override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        int length = 0;
        bool isTopOfWord = true;
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (IsWordSeparator(c))
            {
                if (!isTopOfWord)
                {
                    isTopOfWord = true;
                    length++;
                }
            }
            else
            {
                isTopOfWord = false;
            }
            if (!IsSkipWrite(c))
                length++;
        }
        char[] buf = new char[length];

        isTopOfWord = true;
        int written = 0;
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (IsWordSeparator(c))
            {
                if (!isTopOfWord)
                {
                    isTopOfWord = true;
                    buf[written++] = _separator;
                }
            }
            else
            {
                isTopOfWord = false;
            }
            if (!IsSkipWrite(c))
                buf[written++] = Write(c);
        }
        return new string(buf);
    }
}

public sealed class JsonLowerSnakeCaseNamingPolicy : JsonNamingPolicyBase
{
    public JsonLowerSnakeCaseNamingPolicy() : base('_') { }
    protected override char Write(char c) => char.ToLowerInvariant(c);
}

public sealed class JsonUpperSnakeCaseNamingPolicy : JsonNamingPolicyBase
{
    public JsonUpperSnakeCaseNamingPolicy() : base('_') { }
    protected override char Write(char c) => char.ToUpperInvariant(c);
}

public sealed class JsonKebabCaseNamingPolicy : JsonNamingPolicyBase
{
    public JsonKebabCaseNamingPolicy() : base('-') { }
    protected override char Write(char c) => char.ToLowerInvariant(c);
}
