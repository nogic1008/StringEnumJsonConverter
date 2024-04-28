# Nogic.JsonConverters

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nogic1008/JsonConverters)](https://github.com/nogic1008/JsonConverters/releases)
[![.NET CI](https://github.com/nogic1008/JsonConverters/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nogic1008/JsonConverters/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nogic1008/JsonConverters/branch/master/graph/badge.svg?token=wkwjZuMLHC)](https://codecov.io/gh/nogic1008/JsonConverters)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/JsonConverters/badge)](https://www.codefactor.io/repository/github/nogic1008/JsonConverters)
[![License](https://img.shields.io/github/license/nogic1008/JsonConverters)](LICENSE)

Converters for `System.Text.Json`

## Features

### Converter

For use, see [Register a custom converter](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/converters-how-to#register-a-custom-converter).

#### `DateOnlyConverter`, `TimeOnlyConverter`

> [!IMPORTANT]
> *Obsolete*: Use built-in [`DateOnlyConverter`](https://github.com/dotnet/runtime/blob/v7.0.0/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/DateOnlyConverter.cs) and [`TimeOnlyConverter`](https://github.com/dotnet/runtime/blob/v7.0.0/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/TimeOnlyConverter.cs) on `System.Text.Json@7.0.0` (.NET 7.0) or higher.

Implementation of `JsonConverter<DateOnly>` and `JsonConverter<TimeOnly>`.

#### `EnumStringConverterFactory`, `EnumStringConverter<T>`

Implementation of `JsonConverterFactory` for `enum` that uses `JsonPropertyNameAttribute` and [`EnumMemberAttribute`](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.enummemberattribute)

#### `BlankNullableConverterFactory`, `BlankNullableConverter<T>`

Implementation of `JsonConverterFactory` for `Nullable<T>` that treats `""` as `null`.

### JsonNamingPolicy

> [!IMPORTANT]
> *Obsolete*: Use built-in [`JsonNamingPolicy`](https://learn.microsoft.com/dotnet/api/system.text.json.jsonnamingpolicy) on `System.Text.Json@8.0.0` (.NET 8.0) or higher.

See also [Use a custom JSON property naming policy](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/customize-properties#use-a-custom-json-property-naming-policy).

- `JsonLowerSnakeCaseNamingPolicy`
  - Convert property name to `snake_case`
- `JsonUpperSnakeCaseNamingPolicy`
  - Convert property name to `SNAKE_CASE`
- `JsonKebabCaseNamingPolicy`
  - Convert property name to `kebab-case`
