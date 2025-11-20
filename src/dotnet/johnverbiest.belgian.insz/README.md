# Belgian INSZ/NISS Number Validator

[![NuGet](https://img.shields.io/nuget/v/johnverbiest.belgian.insz.svg)](https://www.nuget.org/packages/johnverbiest.belgian.insz/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

A robust .NET library for validating, parsing, and working with Belgian national identification numbers (INSZ/NISS/RRN/BIS). Provides comprehensive validation, data extraction (birth date, sex, BIS detection), and supports multiple input formats. Thread-safe with zero dependencies.

## 📋 What is INSZ/NISS?

INSZ (Identificatienummer van de Sociale Zekerheid) or NISS (Numéro d'Identification de la Sécurité Sociale), also known as:
- **RRN** (Rijksregisternummer / Numéro de Registre National) - Regular national registry number
- **BIS** (Bis-nummer) - Temporary number for non-residents

These are unique 11-digit identification numbers assigned to individuals in Belgium.

## ✨ Features

- ✅ **Complete Validation** - Validates format, length, checksum, date, and sequence number
- 📅 **Birth Date Extraction** - Extracts birth date and year from valid numbers
- 👤 **Sex Detection** - Determines sex (Male/Female/Unknown) from sequence number
- 🔍 **BIS Detection** - Identifies temporary BIS numbers vs regular RRN
- 🎯 **Type-Safe** - Full .NET type support with comprehensive XML documentation
- 🔄 **Multiple Input Formats** - Accepts strings, long integers, or InszNumber objects
- 📝 **Detailed Error Messages** - Clear validation error reporting
- 🧵 **Thread-Safe** - Safe for concurrent use with lazy-loaded properties
- 🚀 **Zero Dependencies** - Pure .NET Standard 2.0 implementation
- 💉 **DI-Friendly** - Supports both static and instance usage with dependency injection

## 📦 Installation

```bash
dotnet add package johnverbiest.belgian.insz
```

```powershell
Install-Package johnverbiest.belgian.insz
```

## 🚀 Quick Start

```csharp
using johnverbiest.belgian.insz;

// Validate a Belgian national number
var result = InszValidator.ValidateInsz("85073003328");

if (result.IsValid)
{
    Console.WriteLine("Valid INSZ number!");
    Console.WriteLine($"Birth Date: {result.InszNumber?.BirthDate}");
    Console.WriteLine($"Sex: {result.InszNumber?.Sex}");
    Console.WriteLine($"Is BIS: {result.InszNumber?.IsBis}");
}
else
{
    Console.WriteLine($"Invalid: {string.Join(", ", result.ValidationErrors)}");
}
```

## 📖 Usage Examples

### Basic Validation (Static Methods)

```csharp
using johnverbiest.belgian.insz;

// Validate with formatted string
var result1 = InszValidator.ValidateInsz("85.07.30-033.28");
Console.WriteLine(result1.IsValid); // true

// Validate with unformatted string
var result2 = InszValidator.ValidateInsz("85073003328");
Console.WriteLine(result2.IsValid); // true

// Validate with long integer
var result3 = InszValidator.ValidateInsz(85073003328L);
Console.WriteLine(result3.IsValid); // true

// Validate with InszNumber object
var insz = new InszNumber { Value = 85073003328L };
var result4 = InszValidator.ValidateInsz(insz);
Console.WriteLine(result4.IsValid); // true
```

### Dependency Injection Usage

```csharp
using johnverbiest.belgian.insz;
using Microsoft.Extensions.DependencyInjection;

// Register in your DI container (e.g., in Program.cs)
services.AddSingleton<IInszValidator, InszValidator>();

// Inject into your service or controller
public class UserService
{
    private readonly IInszValidator _validator;
    
    public UserService(IInszValidator validator)
    {
        _validator = validator;
    }
    
    public bool ValidateUserInsz(string insz)
    {
        var result = _validator.Validate(insz);
        return result.IsValid;
    }
}
```

### Extracting Information

```csharp
using johnverbiest.belgian.insz;

var result = InszValidator.ValidateInsz("85073003328");

if (result.IsValid && result.InszNumber != null)
{
    var insz = result.InszNumber;
    
    // Access birth information
    Console.WriteLine($"Birth Date: {insz.BirthDate}"); // DateTime object
    Console.WriteLine($"Birth Year: {insz.BirthYear}"); // 1985
    
    // Check sex
    Console.WriteLine($"Sex: {insz.Sex}"); // Male, Female, or Unknown
    if (insz.Sex == Sex.Male)
    {
        Console.WriteLine("This is a male");
    }
    
    // Check if it's a BIS number
    Console.WriteLine($"Is BIS: {insz.IsBis}"); // false (regular RRN)
    
    // Get formatted string
    Console.WriteLine($"Formatted: {insz.ToFormattedString()}"); // "85.07.30-033.28"
    Console.WriteLine($"String: {insz.ToString()}"); // "85073003328"
    Console.WriteLine($"Value: {insz.Value}"); // 85073003328L
}
```

### Working with BIS Numbers

```csharp
using johnverbiest.belgian.insz;

// BIS numbers have month increased by 20 or 40
var result = InszValidator.ValidateInsz("85273003307"); // Month 27 = 07 + 20

if (result.IsValid && result.InszNumber != null)
{
    Console.WriteLine($"Is BIS: {result.InszNumber.IsBis}"); // true
    Console.WriteLine($"Birth Date: {result.InszNumber.BirthDate}"); // July 30, 1985
    Console.WriteLine($"Birth Year: {result.InszNumber.BirthYear}"); // 1985
}
```

### Error Handling

```csharp
using johnverbiest.belgian.insz;

var result = InszValidator.ValidateInsz("12345678901");

if (!result.IsValid)
{
    Console.WriteLine("Validation failed!");
    
    foreach (var error in result.ValidationErrors)
    {
        switch (error)
        {
            case ValidationError.InputIsNotANumber:
                Console.WriteLine("Input contains non-numeric characters");
                break;
            case ValidationError.InputIsWrongLength:
                Console.WriteLine("Input must be exactly 11 digits");
                break;
            case ValidationError.ChecksumIsInvalid:
                Console.WriteLine("Checksum validation failed");
                break;
            case ValidationError.DateIsInvalid:
                Console.WriteLine("Birth date is invalid");
                break;
            case ValidationError.InvalidSequenceNumber:
                Console.WriteLine("Sequence number is out of range");
                break;
        }
    }
}
```

### Async/Await Pattern

```csharp
using johnverbiest.belgian.insz;

public class InszService
{
    private readonly IInszValidator _validator;
    
    public InszService(IInszValidator validator)
    {
        _validator = validator;
    }
    
    public async Task<bool> ValidateInszAsync(string insz)
    {
        // Validation is synchronous and fast, but you can wrap it in Task.Run if needed
        return await Task.Run(() => _validator.Validate(insz).IsValid);
    }
}
```

### ASP.NET Core Integration

```csharp
using johnverbiest.belgian.insz;
using Microsoft.AspNetCore.Mvc;

// In Program.cs
builder.Services.AddSingleton<IInszValidator, InszValidator>();

// In your controller
[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IInszValidator _validator;
    
    public ValidationController(IInszValidator validator)
    {
        _validator = validator;
    }
    
    [HttpPost("validate")]
    public IActionResult ValidateInsz([FromBody] string insz)
    {
        var result = _validator.Validate(insz);
        
        if (result.IsValid)
        {
            return Ok(new
            {
                Valid = true,
                BirthDate = result.InszNumber?.BirthDate,
                Sex = result.InszNumber?.Sex?.ToString(),
                IsBis = result.InszNumber?.IsBis,
                Formatted = result.InszNumber?.ToFormattedString()
            });
        }
        
        return BadRequest(new
        {
            Valid = false,
            Errors = result.ValidationErrors.Select(e => e.ToString()).ToArray()
        });
    }
}
```

## 🔧 API Reference

### InszValidator Class

The main validator class providing both static and instance methods.

#### Static Methods

| Method | Description |
|--------|-------------|
| `ValidateInsz(string)` | Validates an INSZ number from a string. Accepts formatted or unformatted input. |
| `ValidateInsz(long)` | Validates an INSZ number from a long integer. |
| `ValidateInsz(InszNumber)` | Validates an INSZ number from an InszNumber object. |

#### Instance Methods (IInszValidator)

| Method | Description |
|--------|-------------|
| `Validate(string)` | Validates an INSZ number from a string. |
| `Validate(long)` | Validates an INSZ number from a long integer. |
| `Validate(InszNumber)` | Validates an INSZ number from an InszNumber object. |

### InszValidationResult Class

Represents the result of an INSZ validation operation.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsValid` | `bool` | Indicates whether the INSZ number is valid. |
| `InszNumber` | `InszNumber?` | The validated INSZ number object (null if invalid). |
| `ValidationErrors` | `ValidationError[]` | Array of validation errors if the number is invalid. |

### InszNumber Record

Represents a Belgian INSZ number with extracted information.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `long` | The numeric value of the INSZ (11 digits). |
| `HasBeenValidated` | `bool` | Whether this number has been validated. |
| `IsValid` | `bool?` | Validation result. |
| `IsBis` | `bool?` | Whether this is a BIS number. |
| `BirthDate` | `DateTime?` | Extracted birth date. |
| `BirthYear` | `int?` | Extracted birth year. |
| `Sex` | `Sex?` | Extracted sex. |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ToString()` | `string` | Returns the 11-digit string representation. |
| `ToFormattedString()` | `string` | Returns formatted string (XX.XX.XX-XXX.XX). |

### Sex Enum

```csharp
public enum Sex
{
    Unknown = 0,
    Female = 1,
    Male = 2
}
```

### ValidationError Enum

```csharp
public enum ValidationError
{
    InputIsNotANumber,
    ChecksumIsInvalid,
    InputIsWrongLength,
    DateIsInvalid,
    InvalidSequenceNumber
}
```

## 🎯 INSZ Number Format

An INSZ number consists of 11 digits with the following structure:

```
YY MM DD SSS CC
│  │  │  │   └─ Check digits (2 digits)
│  │  │  └───── Sequence number (3 digits, 001-998)
│  │  └──────── Day of birth (2 digits)
│  └─────────── Month of birth or month placeholder (2 digits)
└────────────── Year of birth (2 digits)
```

### Month Encoding

- **00-12**: Regular RRN with known sex
- **20-32**: BIS number with known sex (month + 20)
- **40-52**: Regular RRN with unknown sex (month + 40)
- **60-72**: BIS number with unknown sex (month + 60)

### Sequence Number

- **Odd numbers (001, 003, 005, ...)**: Male
- **Even numbers (002, 004, 006, ...)**: Female
- **Range**: 001-998

### Checksum

The last 2 digits are calculated using modulo 97:
- For dates before 2000: `97 - (first 9 digits % 97)`
- For dates after 2000: `97 - ((2 + first 9 digits) % 97)`

## 🎓 Framework Support

This library targets **.NET Standard 2.0**, which means it's compatible with:

- ✅ .NET Core 2.0 and later
- ✅ .NET Framework 4.6.1 and later
- ✅ .NET 5, 6, 7, 8, 9+
- ✅ Mono 5.4 and later
- ✅ Xamarin.iOS 10.14 and later
- ✅ Xamarin.Android 8.0 and later
- ✅ Universal Windows Platform 10.0.16299 and later

## 📚 Additional Resources

- [Official Belgian Government Documentation](https://www.ibz.rrn.fgov.be/)
- [INSZ Format Specification](https://www.ksz-bcss.fgov.be/nl/insz)
- [National Registry Information](https://www.ibz.rrn.fgov.be/nl/rijksregister/)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](../../../LICENSE) file for details.

## 👨‍💻 Author

**John Verbiest**

## 🐛 Issues

Found a bug or have a feature request? Please open an issue on [GitHub](https://github.com/johnverbiest/johnverbiest.belgian.insz/issues).

## 🔗 Related Packages

- [npm Version](https://www.npmjs.com/package/johnverbiest.belgian.insz) - TypeScript/JavaScript package for Node.js and browser applications

---

Made with ❤️ in Belgium

```csharp
using johnverbiest.belgian.insz;

// Validate from string (with or without formatting)
var result = InszValidator.ValidateInsz("85.07.30-033.28");

if (result.IsValid)
{
    Console.WriteLine("✓ Valid INSZ number");
    var insz = result.InszNumber;
    Console.WriteLine($"Birth Year: {insz.BirthYear}");
    Console.WriteLine($"Birth Date: {insz.BirthDate?.ToShortDateString()}");
    Console.WriteLine($"Formatted: {insz.ToFormattedString()}");
}
else
{
    Console.WriteLine("✗ Invalid INSZ number");
    foreach (var error in result.ValidationErrors)
    {
        Console.WriteLine($"  - {error}");
    }
}

// Validate from long integer
var result2 = InszValidator.ValidateInsz(85073003328L);

// Validate from InszNumber object
var inszNumber = new InszNumber { Value = 85073003328L };
var result3 = InszValidator.ValidateInsz(inszNumber);
```

### Instance Usage (Dependency Injection)

For applications using dependency injection, create an instance of `InszValidator` and use the interface:

```csharp
using johnverbiest.belgian.insz;

// Register in your DI container (e.g., in Startup.cs or Program.cs)
services.AddSingleton<IInszValidator, InszValidator>();

// Inject into your service or controller
public class MyService
{
    private readonly IInszValidator _validator;
    
    public MyService(IInszValidator validator)
    {
        _validator = validator;
    }
    
    public bool ValidateUserInsz(string insz)
    {
        var result = _validator.Validate(insz);
        return result.IsValid;
    }
    
    public InszValidationResult GetValidationDetails(long inszNumber)
    {
        return _validator.Validate(inszNumber);
    }
}
```

## 🔍 API Reference

### InszValidator Class

The main validator class providing both static and instance methods.

#### Static Methods

| Method | Description |
|--------|-------------|
| `ValidateInsz(string)` | Validates an INSZ number from a string. Accepts formatted or unformatted input. |
| `ValidateInsz(long)` | Validates an INSZ number from a long integer. |
| `ValidateInsz(InszNumber)` | Validates an INSZ number from an InszNumber object. |

#### Instance Methods (IInszValidator)

| Method | Description |
|--------|-------------|
| `Validate(string)` | Validates an INSZ number from a string. Delegates to static method. |
| `Validate(long)` | Validates an INSZ number from a long integer. Delegates to static method. |
| `Validate(InszNumber)` | Validates an INSZ number from an InszNumber object. Delegates to static method. |

### InszValidationResult Class

Represents the result of an INSZ validation operation.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsValid` | `bool` | Indicates whether the INSZ number is valid. |
| `InszNumber` | `InszNumber?` | The validated INSZ number object (null if invalid). |
| `ValidationErrors` | `ValidationError[]` | Array of validation errors if the number is invalid. |

### InszNumber Record

Represents a validated Belgian INSZ number with parsed components.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `long` | The raw 11-digit INSZ number as a long integer. |
| `HasBeenValidated` | `bool` | Indicates if the number has been processed through validation. |
| `IsValid` | `bool?` | Validation status: true if valid, false if invalid, null if not validated. |
| `BirthDate` | `DateTime?` | The extracted birth date (null if invalid or unknown). |
| `BirthYear` | `int?` | The extracted birth year (null if invalid or unknown). |
| `IsBis` | `bool?` | Indicates if this is a BIS number (null if not validated). |
| `Sex` | `Sex?` | The determined sex (Male/Female/Unknown, null if not validated). |

#### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `ToString()` | `string` | Returns the unformatted 11-digit INSZ number. |
| `ToFormattedString()` | `string?` | Returns the formatted INSZ number (YY.MM.DD-NNN.CC) or null if invalid. |

### ValidationError Enum

Enumeration of possible validation errors.

| Value | Description |
|-------|-------------|
| `InputIsNotANumber` | The input contains non-numeric characters. |
| `ChecksumIsInvalid` | The checksum validation failed. |
| `InputIsWrongLength` | The input is not exactly 11 digits. |
| `DateIsInvalid` | The date portion is not a valid date. |
| `InvalidSequenceNumber` | The sequence number is 000 or 999 (reserved values). |

### Sex Enum

Enumeration representing the sex/gender extracted from an INSZ number.

| Value | Description |
|-------|-------------|
| `Unknown` | Sex cannot be determined (regular INSZ or BIS with unknown sex). |
| `Female` | Female (BIS numbers with known sex, even sequence number). |
| `Male` | Male (BIS numbers with known sex, odd sequence number). |

## 💡 Detailed Examples

### Example 1: Validating Different Input Formats

The validator accepts multiple input formats and automatically strips common formatting characters:

```csharp
var validator = new InszValidator();

// All of these are valid and equivalent:
validator.Validate("85.07.30-033.28");  // Formatted
validator.Validate("85 07 30 033 28");  // Spaces
validator.Validate("85-07-30-033-28");  // Dashes
validator.Validate("85073003328");      // Unformatted
validator.Validate(85073003328L);       // Long integer
```

### Example 2: Extracting Information

Once validated, you can extract various information from the INSZ number:

```csharp
var result = InszValidator.ValidateInsz("85073003328");

if (result.IsValid)
{
    var insz = result.InszNumber;
    
    // Basic information
    Console.WriteLine($"Raw Value: {insz.Value}");              // 85073003328
    Console.WriteLine($"String: {insz.ToString()}");            // "85073003328"
    Console.WriteLine($"Formatted: {insz.ToFormattedString()}"); // "85.07.30-033.28"
    
    // Date information
    Console.WriteLine($"Birth Date: {insz.BirthDate}");         // 30/07/1985
    Console.WriteLine($"Birth Year: {insz.BirthYear}");         // 1985
    
    // Type and sex
    Console.WriteLine($"Is BIS Number: {insz.IsBis}");          // false
    Console.WriteLine($"Sex: {insz.Sex}");                       // Unknown (for regular numbers)
    
    // Validation status
    Console.WriteLine($"Is Valid: {insz.IsValid}");              // true
    Console.WriteLine($"Has Been Validated: {insz.HasBeenValidated}"); // true
}
```

### Example 3: Working with BIS Numbers

BIS numbers are special identifiers for individuals not registered in the Belgian population register:

```csharp
// BIS number with known sex (month + 40)
var result1 = InszValidator.ValidateInsz("85473003312");

if (result1.IsValid)
{
    var insz = result1.InszNumber;
    Console.WriteLine($"Is BIS: {insz.IsBis}");        // true
    Console.WriteLine($"Sex: {insz.Sex}");             // Male (odd sequence number)
    Console.WriteLine($"Birth Date: {insz.BirthDate}"); // Adjusted for BIS format
}

// BIS number with unknown sex (month + 20)
var result2 = InszValidator.ValidateInsz("85273003330");

if (result2.IsValid)
{
    var insz = result2.InszNumber;
    Console.WriteLine($"Is BIS: {insz.IsBis}");        // true
    Console.WriteLine($"Sex: {insz.Sex}");             // Unknown
}
```

### Example 4: Comprehensive Error Handling

The library provides detailed validation errors:

```csharp
var result = InszValidator.ValidateInsz("12345678901");

if (!result.IsValid)
{
    Console.WriteLine("Validation failed with the following errors:");
    
    foreach (var error in result.ValidationErrors)
    {
        switch (error)
        {
            case ValidationError.InputIsNotANumber:
                Console.WriteLine("- Input contains non-numeric characters");
                break;
            case ValidationError.ChecksumIsInvalid:
                Console.WriteLine("- Checksum validation failed");
                break;
            case ValidationError.InputIsWrongLength:
                Console.WriteLine("- Input must be exactly 11 digits");
                break;
            case ValidationError.DateIsInvalid:
                Console.WriteLine("- Date is not valid");
                break;
            case ValidationError.InvalidSequenceNumber:
                Console.WriteLine("- Sequence number is invalid (000 or 999)");
                break;
        }
    }
}
```

### Example 5: Form Validation

Integrate INSZ validation into your application forms:

```csharp
public class PersonForm
{
    public string InszNumber { get; set; }
    public string Name { get; set; }
}

public class PersonValidator
{
    private readonly IInszValidator _inszValidator;
    
    public PersonValidator(IInszValidator inszValidator)
    {
        _inszValidator = inszValidator;
    }
    
    public ValidationResult ValidateForm(PersonForm form)
    {
        var errors = new List<string>();
        
        // Validate INSZ number
        var inszResult = _inszValidator.Validate(form.InszNumber);
        if (!inszResult.IsValid)
        {
            var errorMessages = string.Join(", ", inszResult.ValidationErrors);
            errors.Add($"Invalid INSZ: {errorMessages}");
        }
        
        // Other validations...
        if (string.IsNullOrWhiteSpace(form.Name))
        {
            errors.Add("Name is required");
        }
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}
```

### Example 6: Data Processing and Enrichment

Automatically extract and enrich person data from INSZ numbers:

```csharp
public class Person
{
    public long InszValue { get; set; }
    public string FormattedInsz { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? Age { get; set; }
    public string Sex { get; set; }
    public bool IsBisNumber { get; set; }
}

public Person CreatePersonFromInsz(long inszValue)
{
    var result = InszValidator.ValidateInsz(inszValue);
    
    if (!result.IsValid)
        throw new ArgumentException("Invalid INSZ number", nameof(inszValue));
    
    var insz = result.InszNumber;
    
    return new Person
    {
        InszValue = insz.Value,
        FormattedInsz = insz.ToFormattedString(),
        BirthDate = insz.BirthDate,
        Age = insz.BirthDate.HasValue 
            ? DateTime.Today.Year - insz.BirthDate.Value.Year 
            : null,
        Sex = insz.Sex?.ToString() ?? "Unknown",
        IsBisNumber = insz.IsBis ?? false
    };
}
```

### Example 7: Batch Validation

Validate multiple INSZ numbers efficiently:

```csharp
public class BatchValidator
{
    private readonly IInszValidator _validator;
    
    public BatchValidator(IInszValidator validator)
    {
        _validator = validator;
    }
    
    public BatchValidationResult ValidateBatch(List<string> inszNumbers)
    {
        var results = new List<(string Insz, bool IsValid, string[] Errors)>();
        
        foreach (var insz in inszNumbers)
        {
            var result = _validator.Validate(insz);
            results.Add((
                insz, 
                result.IsValid,
                result.ValidationErrors.Select(e => e.ToString()).ToArray()
            ));
        }
        
        return new BatchValidationResult
        {
            TotalCount = results.Count,
            ValidCount = results.Count(r => r.IsValid),
            InvalidCount = results.Count(r => !r.IsValid),
            Results = results
        };
    }
}

// Usage with static method for simple scenarios
var inszNumbers = new[] { "85073003328", "12345678901", "00010112345" };
var validNumbers = inszNumbers
    .Where(insz => InszValidator.ValidateInsz(insz).IsValid)
    .ToList();
```

### Example 8: Custom Validation Logic

Extend the validator with custom business rules:

```csharp
public class CustomInszValidator : IInszValidator
{
    private readonly InszValidator _baseValidator = new();
    
    public InszValidationResult Validate(string inszString)
    {
        var result = _baseValidator.Validate(inszString);
        
        // Add custom validation: reject if person is under 18
        if (result.IsValid && result.InszNumber?.BirthDate != null)
        {
            var age = DateTime.Today.Year - result.InszNumber.BirthDate.Value.Year;
            if (age < 18)
            {
                // You could create a custom result type here
                Console.WriteLine("Warning: Person is under 18 years old");
            }
        }
        
        return result;
    }
    
    public InszValidationResult Validate(long inszNumber) 
        => Validate(inszNumber.ToString("D11"));
    
    public InszValidationResult Validate(InszNumber inszNumber) 
        => Validate(inszNumber.Value);
}
```

## ✅ Validation Rules

The validator performs the following checks:

1. **Format Validation**: Must be 11 numeric digits (formatting characters are automatically stripped)
2. **Checksum Validation**: Modulo 97 validation with support for both pre-2000 and post-2000 birth dates
3. **Date Validation**: The first 6 digits must form a valid date (with special handling for BIS numbers)
4. **Sequence Number Validation**: Must be between 001 and 998 (000 and 999 are reserved)
5. **BIS Number Detection**: Correctly identifies and validates BIS numbers with modified month values

### Special Cases

- **BIS Numbers with Unknown Date**: Month 00, Day 00 (birth date unknown but valid)
- **BIS Numbers with Unknown Year**: Year 00, specific day patterns (partial date information)
- **Post-2000 Dates**: Automatic detection based on checksum calculation

## 🎯 Features

- ✅ Validates both regular INSZ and BIS numbers
- ✅ Supports multiple input formats (string, long, formatted)
- ✅ Static methods for simple scenarios
- ✅ Instance methods for dependency injection
- ✅ Extracts birth date and year with century detection
- ✅ Determines sex from sequence number (where applicable)
- ✅ Provides detailed validation error messages
- ✅ Thread-safe with lazy-loaded properties
- ✅ Supports .NET 8.0, .NET 6.0, and .NET Standard 2.0
- ✅ Comprehensive XML documentation
- ✅ Zero external dependencies

## 🏗️ Framework Support

- **.NET 8.0** - Latest LTS version with all features
- **.NET 6.0** - LTS version with full compatibility
- **.NET Standard 2.0** - Compatible with .NET Framework 4.6.1+, Xamarin, Unity, and more

## 🔧 Advanced Topics

### Thread Safety

All validation operations are thread-safe. The `InszNumber` record uses lazy-loaded properties with thread-safe initialization, making it safe to validate INSZ numbers concurrently.

```csharp
var tasks = inszNumbers.Select(insz => 
    Task.Run(() => InszValidator.ValidateInsz(insz))
);
var results = await Task.WhenAll(tasks);
```

### Performance Considerations

- Static methods have no instantiation overhead
- Lazy-loaded properties in `InszNumber` ensure efficient memory usage
- Validation is fast: typically < 1ms per number

### Null Safety

The library is designed with C# nullable reference types enabled:
- All nullable returns are explicitly marked
- Properties return `null` for invalid or unvalidated numbers
- Use null-conditional operators when accessing properties

```csharp
var result = InszValidator.ValidateInsz("85073003328");
var birthYear = result.InszNumber?.BirthYear ?? 0; // Safe access
```

## 📚 Additional Resources

- [Belgian National Register Rules](../../../docs/BelgiumRegistryRules.md)
- [Test Vectors](../../../test-vectors/README.md)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the Apache License 2.0

## 🔗 Links

- [GitHub Repository](https://github.com/johnverbiest/johnverbiest.belgian.insz)
- [NuGet Package](https://www.nuget.org/packages/johnVerbiest.belgium.insz)
- [Report Issues](https://github.com/johnverbiest/johnverbiest.belgian.insz/issues)

---

Made with ❤️ in Belgium

