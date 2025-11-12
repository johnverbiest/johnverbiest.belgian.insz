# Belgian INSZ/NISS Number Validator

A comprehensive .NET library for validating and parsing Belgian National Numbers (INSZ/NISS - Identificatienummer van de Sociale Zekerheid/Numéro d'identification de la sécurité sociale) and BIS numbers.

## Features

✅ **Complete INSZ Validation** - Validates format, length, checksum, and date components  
✅ **BIS Number Support** - Full support for Belgian BIS (Bis-nummer) identification numbers  
✅ **Data Extraction** - Extract birth date, birth year, and sex from valid numbers  
✅ **Multiple Input Formats** - Accept string, long, or strongly-typed InszNumber inputs  
✅ **Thread-Safe** - All operations are thread-safe with lazy-loaded properties  
✅ **Cross-Platform** - Supports .NET 6.0, .NET 8.0, and .NET Standard 2.0  

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package johnverbiest.belgian.insz
```

Or via Package Manager Console:

```powershell
Install-Package johnverbiest.belgian.insz
```

## Quick Start

### Basic Validation

```csharp
using johnverbiest.belgian.insz;

var validator = new InszValidator();

// Validate a string
var result = validator.Validate("85073003328");
if (result.IsValid)
{
    Console.WriteLine($"Valid INSZ number: {result.InszNumber.ToFormattedString()}");
    Console.WriteLine($"Birth year: {result.InszNumber.BirthYear}");
    Console.WriteLine($"Sex: {result.InszNumber.Sex}");
}

// Validate a long
var result2 = validator.Validate(85073003328L);

// Validate an InszNumber object
var inszNumber = new InszNumber { Value = 85073003328L };
var result3 = validator.Validate(inszNumber);
```

### Working with Validation Results

```csharp
var validator = new InszValidator();
var result = validator.Validate("invalid-number");

if (!result.IsValid)
{
    Console.WriteLine("Validation failed:");
    foreach (var error in result.ValidationErrors)
    {
        Console.WriteLine($"- {error}");
    }
}
```

### Extracting Information

```csharp
var validator = new InszValidator();
var result = validator.Validate("85073003328");

if (result.IsValid)
{
    var insz = result.InszNumber;
    
    Console.WriteLine($"Raw number: {insz}");                    // 85073003328
    Console.WriteLine($"Formatted: {insz.ToFormattedString()}"); // 85.07.30-033.28
    Console.WriteLine($"Birth date: {insz.BirthDate}");          // 30/07/1985
    Console.WriteLine($"Birth year: {insz.BirthYear}");          // 1985
    Console.WriteLine($"Is BIS number: {insz.IsBis}");           // False
    Console.WriteLine($"Sex: {insz.Sex}");                       // Unknown (for regular INSZ)
}
```

## Understanding INSZ Numbers

### Format Structure

INSZ numbers follow the format: **YYMMDD-NNN-CC**

- **YY**: Birth year (last two digits)
- **MM**: Birth month (01-12, modified for BIS numbers)
- **DD**: Birth day (01-31)
- **NNN**: Sequence number (001-998)
- **CC**: Check digits (modulo 97 checksum)

### Century Determination

The library automatically determines the correct century (1900s vs 2000s) using the checksum validation method.

### BIS Numbers

BIS (Bis-nummer) numbers are special identification numbers for individuals not registered in the Belgian population register:

- **Month +20**: BIS number with unknown sex (e.g., month 23 = March with unknown sex)
- **Month +40**: BIS number with known sex (e.g., month 43 = March with known sex)

For BIS numbers with known sex, the sequence number determines gender (even = female, odd = male).

## Validation Rules

The validator checks for:

1. **Format**: Must be numeric and exactly 11 digits
2. **Length**: Must be exactly 11 characters
3. **Checksum**: Must pass modulo 97 validation
4. **Date**: Must represent a valid date (with special handling for BIS numbers)
5. **Sequence**: Sequence number cannot be 000 or 999

## API Reference

### InszValidator Class

#### Methods

```csharp
InszValidationResult Validate(string inszString)
InszValidationResult Validate(long inszNumber)
InszValidationResult Validate(InszNumber inszNumber)
```

#### Static Helper Methods

```csharp
static DateTime? GetDate(string inszString)
static int? GetBirthYear(string inszString)
static Sex? GetSex(string inszString)
static bool IsBisNumber(string inszString)
static bool IsBisWithSexKnown(string inszString)
static bool IsBisWithSexUnknown(string inszString)
```

### InszNumber Record

#### Properties

- `long Value` - The numeric INSZ value
- `bool HasBeenValidated` - Whether validation has been performed
- `bool? IsValid` - Validation result (null if not validated)
- `bool? IsBis` - Whether this is a BIS number
- `DateTime? BirthDate` - Extracted birth date
- `int? BirthYear` - Extracted birth year
- `Sex? Sex` - Determined sex/gender

#### Methods

- `string ToString()` - Returns unformatted 11-digit string
- `string? ToFormattedString()` - Returns formatted string (YY.MM.DD-NNN.CC)

### InszValidationResult Class

#### Properties

- `bool IsValid` - Whether the number is valid
- `InszNumber? InszNumber` - The validated number (if valid)
- `ValidationError[] ValidationErrors` - Array of validation errors

### Enums

#### Sex
- `Male`
- `Female` 
- `Unknown`

#### ValidationError
- `InputIsNotANumber`
- `InputIsWrongLength`
- `ChecksumIsInvalid`
- `DateIsInvalid`
- `InvalidSequenceNumber`

## Examples

### Handling Different Input Formats

```csharp
var validator = new InszValidator();

// With spaces and dashes (automatically cleaned)
var result1 = validator.Validate("85.07.30-033.28");

// As pure number string
var result2 = validator.Validate("85073003328");

// As long integer
var result3 = validator.Validate(85073003328L);

// All produce the same result
Console.WriteLine(result1.IsValid == result2.IsValid == result3.IsValid); // True
```

### Working with BIS Numbers

```csharp
var validator = new InszValidator();

// BIS number with known sex
var bisResult = validator.Validate("85473003384"); // Month 47 = July + 40

if (bisResult.IsValid)
{
    var bis = bisResult.InszNumber;
    Console.WriteLine($"Is BIS: {bis.IsBis}");           // True
    Console.WriteLine($"Sex: {bis.Sex}");                // Male (odd sequence number)
    Console.WriteLine($"Birth date: {bis.BirthDate}");   // 30/07/1985
}
```

### Error Handling

```csharp
var validator = new InszValidator();
var result = validator.Validate("12345");

if (!result.IsValid)
{
    foreach (var error in result.ValidationErrors)
    {
        switch (error)
        {
            case ValidationError.InputIsWrongLength:
                Console.WriteLine("Number must be 11 digits long");
                break;
            case ValidationError.ChecksumIsInvalid:
                Console.WriteLine("Invalid checksum");
                break;
            case ValidationError.DateIsInvalid:
                Console.WriteLine("Invalid birth date");
                break;
            // ... handle other errors
        }
    }
}
```

## Thread Safety

All classes and methods in this library are thread-safe. The `InszNumber` record uses lazy-loaded properties with thread-safe initialization, making it safe to use in multi-threaded environments.

## Performance

- Lazy-loaded properties ensure efficient memory usage
- Validation is performed once and cached
- String operations are optimized for performance
- No external dependencies for core functionality

## Compatibility

- **.NET 6.0** and higher
- **.NET 8.0** and higher  
- **.NET Standard 2.0** (compatible with .NET Framework 4.6.1+)

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

If you encounter any issues or have questions, please:

1. Check the documentation and examples above
2. Search existing issues on GitHub
3. Create a new issue with detailed information about your problem

---

**Belgian INSZ - NISS Validator** - Making Belgian national number validation simple and reliable.
