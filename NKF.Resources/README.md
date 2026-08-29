# NKF.Resources

Shared resources: DataDictionary and localized messages.

## Features

- **DataDictionary** — Localized entity/property names for UI
- **ErrorMessages / FramWorkErrorMessages** — Validation error messages with formatting (`DuplicateValue`)
- **Validations / FrameWorkValidations** — Business rule validation messages (`Required`, `MaxLength`, etc.)
- **DomainErrors.Duplicate** — Helper in `NKF.Domain` consuming `FramWorkErrorMessages.DuplicateValue` (1.0.1)

## Install

```bash
dotnet add package NKF.Resources
```

## Usage

```csharp
// In domain (via DomainErrors helper, 1.0.1)
IError error = DomainErrors.Duplicate(nameof(Brand), DataDictionary.Brand);
return Result.Fail(error);
// Or raw
string msg = string.Format(FramWorkErrorMessages.DuplicateValue, DataDictionary.BrandName);

// Resources are embedded and accessible via:
// NKF.Resources.DataDictionary.BrandName
// NKF.Resources.Messages.FramWorkErrorMessages.DuplicateValue
// NKF.Resources.Messages.FrameWorkValidations.Required
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0