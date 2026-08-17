# NKF.Resources

Shared resources: DataDictionary and localized messages.

## Features

- **DataDictionary** — Localized entity/property names for UI
- **ErrorMessages** — Validation error messages with formatting
- **Validations** — Business rule validation messages

## Install

```bash
dotnet add package NKF.Resources
```

## Usage

```csharp
// In domain
string error = string.Format(ErrorMessages.DuplicateValue, DataDictionary.BrandName, name);

// Resources are embedded and accessible via:
// NKF.Resources.DataDictionary.BrandName
// NKF.Resources.Messages.ErrorMessages.DuplicateValue
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0