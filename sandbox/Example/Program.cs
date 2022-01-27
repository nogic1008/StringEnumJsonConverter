using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nogic.JsonConverters;

var options = new JsonSerializerOptions()
{
    Converters = { new DateOnlyConverter(), new TimeOnlyConverter(), new EnumStringConverterFactory() }
};

Console.WriteLine($"---------- {nameof(DateOnlyConverter)} ----------");
ConvertJson(DateOnly.FromDateTime(DateTime.Now), "\"2022-01-10\"");

Console.WriteLine($"---------- {nameof(TimeOnlyConverter)} ----------");
ConvertJson(TimeOnly.FromDateTime(DateTime.Now), "\"22:15:20.300\"");

Console.WriteLine($"---------- {nameof(EnumStringConverterFactory)} ----------");
ConvertJson(Status.None, "\"None\"");
ConvertJson(Status.Running, "1");
ConvertJson(Status.FatalError, "\"fatal_error\"");

Console.WriteLine($"---------- {nameof(JsonLowerSnakeCaseNamingPolicy)} ----------");
options = new() { PropertyNamingPolicy = new JsonLowerSnakeCaseNamingPolicy() };
string json = "{\"id\":2, \"name\":\"User 2\", \"mail_address\":\"user02@example.com\"}";
ConvertJson(new User(1, "User 1", "user01@example.com"), json);

Console.WriteLine($"---------- {nameof(JsonUpperSnakeCaseNamingPolicy)} ----------");
options = new() { PropertyNamingPolicy = new JsonUpperSnakeCaseNamingPolicy() };
json = "{\"ID\":4, \"NAME\":\"User 4\", \"MAIL_ADDRESS\":\"user04@example.com\"}";
ConvertJson(new User(3, "User 3", "user03@example.com"), json);

Console.WriteLine($"---------- {nameof(JsonKebabCaseNamingPolicy)} ----------");
options = new() { PropertyNamingPolicy = new JsonKebabCaseNamingPolicy() };
json = "{\"id\":6, \"name\":\"User 6\", \"mail-address\":\"user06@example.com\"}";
ConvertJson(new User(5, "User 5", "user05@example.com"), json);

void ConvertJson<T>(T value, string json, [CallerArgumentExpression("value")] string? arg = null)
{
    string typeName = typeof(T).ToString().Split('.')[^1];
    Console.WriteLine($"Serialize({arg})");
    Console.WriteLine($"  => {JsonSerializer.Serialize(value, options)}");
    Console.WriteLine($"Deserialize<{typeName}>({json})");
    Console.WriteLine($"  => {JsonSerializer.Deserialize<T>(json, options)}");
    Console.WriteLine();
}

internal enum Status
{
    None = 0,
    [EnumMember(Value = "running")] Running = 1,
    [JsonPropertyName("fatal_error")] FatalError = 2,
}

internal record User(int Id, string Name, string MailAddress);
