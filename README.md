# Nogic.JsonConverters

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nogic1008/JsonConverters)](https://github.com/nogic1008/JsonConverters/releases)
[![.NET CI](https://github.com/nogic1008/JsonConverters/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nogic1008/JsonConverters/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nogic1008/JsonConverters/branch/master/graph/badge.svg?token=wkwjZuMLHC)](https://codecov.io/gh/nogic1008/JsonConverters)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/JsonConverters/badge)](https://www.codefactor.io/repository/github/nogic1008/JsonConverters)
[![License](https://img.shields.io/github/license/nogic1008/JsonConverters)](LICENSE)

Converters for `System.Text.Json`

## Features

### Converter

For use, see [Register a custom converter](https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-converters-how-to#register-a-custom-converter).

#### `DateOnlyConverter`, `TimeOnlyConverter`

Implementation of `JsonConverter<DateOnly>` and `JsonConverter<TimeOnly>`.

#### `EnumStringConverterFactory`, `EnumStringConverter<T>`

Implementation of `JsonConverterFactory` for `enum` that uses `JsonPropertyNameAttribute` and [`EnumMemberAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.serialization.enummemberattribute)

#### `BlankNullableConverterFactory`, `BlankNullableConverter<T>`

Implementation of `JsonConverterFactory` for `Nullable<T>` that treats `""` as `null`.

### JsonNamingPolicy

See also [Use a custom JSON property naming policy](https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-customize-properties#use-a-custom-json-property-naming-policy).

- `JsonLowerSnakeCaseNamingPolicy`
  - Convert property name to `snake_case`
- `JsonUpperSnakeCaseNamingPolicy`
  - Convert property name to `SNAKE_CASE`
- `JsonKebabCaseNamingPolicy`
  - Convert property name to `kebab-case`
