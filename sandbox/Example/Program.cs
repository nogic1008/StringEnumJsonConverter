using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json;
using Nogic.JsonConverters;

var options = new JsonSerializerOptions()
{
    Converters = { new DateOnlyConverter(), new TimeOnlyConverter(), new EnumStringConverter() }
};

ConvertJson(nameof(DateOnlyConverter), DateOnly.FromDateTime(DateTime.Now), "\"2022-01-10\"");
ConvertJson(nameof(TimeOnlyConverter), TimeOnly.FromDateTime(DateTime.Now), "\"22:15:20.300\"");

ConvertJson(nameof(EnumStringConverter), Status.None, "\"None\"");
ConvertJson(nameof(EnumStringConverter), Status.Running, "1");
ConvertJson(nameof(EnumStringConverter), Status.FatalError, "\"fatal_error\"");

options = new() { PropertyNamingPolicy = new JsonLowerSnakeCaseNamingPolicy() };
string json = "{\"id\":2, \"name\":\"User 2\", \"mail_address\":\"user02@example.com\"}";
ConvertJson(nameof(JsonLowerSnakeCaseNamingPolicy), new User(1, "User 1", "user01@example.com"), json);

options = new() { PropertyNamingPolicy = new JsonUpperSnakeCaseNamingPolicy() };
json = "{\"ID\":4, \"NAME\":\"User 4\", \"MAIL_ADDRESS\":\"user04@example.com\"}";
ConvertJson(nameof(JsonUpperSnakeCaseNamingPolicy), new User(3, "User 3", "user03@example.com"), json);

options = new() { PropertyNamingPolicy = new JsonKebabCaseNamingPolicy() };
json = "{\"id\":6, \"name\":\"User 6\", \"mail-address\":\"user06@example.com\"}";
ConvertJson(nameof(JsonKebabCaseNamingPolicy), new User(5, "User 5", "user05@example.com"), json);

void ConvertJson<T>(string section, T value, string json, [CallerArgumentExpression("value")] string? arg = null)
{
    string typeName = typeof(T).ToString().Split('.')[^1];
    Console.WriteLine($"---------- {section} ----------");
    Console.WriteLine($"Serialize({arg}) = {JsonSerializer.Serialize(value, options)}");
    Console.WriteLine($"Deserialize<{typeName}>({json}) = {JsonSerializer.Deserialize<T>(json, options)}");
    Console.WriteLine($"---------- End of {section} ----------");
    Console.WriteLine();
}

internal enum Status
{
    None = 0,
    [EnumMember(Value = "running")] Running = 1,
    [EnumMember(Value = "fatal_error")] FatalError = 2,
}

internal record User(int Id, string Name, string MailAddress);
