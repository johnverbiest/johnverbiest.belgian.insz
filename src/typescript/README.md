# Belgian INSZ/NISS Number Validator

[![npm version](https://badge.fury.io/js/johnverbiest.belgian.insz.svg)](https://www.npmjs.com/package/johnverbiest.belgian.insz)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

A robust TypeScript library for validating, parsing, and working with Belgian national identification numbers (INSZ/NISS/RRN/BIS). Provides comprehensive validation, data extraction (birth date, sex, BIS detection), and supports multiple input formats.

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
- 🎯 **Type-Safe** - Full TypeScript support with comprehensive type definitions
- 🔄 **Multiple Input Formats** - Accepts strings, numbers, or InszNumber objects
- 📝 **Detailed Error Messages** - Clear validation error reporting

## 📦 Installation

```bash
npm install johnverbiest.belgian.insz
```

```bash
yarn add johnverbiest.belgian.insz
```

```bash
pnpm add johnverbiest.belgian.insz
```

## 🚀 Quick Start

```typescript
import { InszValidator } from 'johnverbiest.belgian.insz';

const validator = new InszValidator();

// Validate a Belgian national number
const result = validator.Validate('85073003328');

if (result.isValid) {
  console.log('Valid INSZ number!');
  console.log('Birth Date:', result.inszNumber?.birthDate);
  console.log('Sex:', result.inszNumber?.sex);
  console.log('Is BIS:', result.inszNumber?.isBis);
} else {
  console.log('Invalid:', result.validationErrors);
}
```

## 📖 Usage Examples

### Basic Validation

```typescript
import { InszValidator } from 'johnverbiest.belgian.insz';

const validator = new InszValidator();

// Validate with string
const result1 = validator.Validate('85073003328');
console.log(result1.isValid); // true

// Validate with number
const result2 = validator.Validate(85073003328);
console.log(result2.isValid); // true

// Static method validation
const result3 = InszValidator.Validate('00000000097');
console.log(result3.isValid); // true
```

### Extracting Information

```typescript
import { InszValidator, Sex } from 'johnverbiest.belgian.insz';

const result = InszValidator.Validate('85073003328');

if (result.isValid && result.inszNumber) {
  const insz = result.inszNumber;
  
  // Access birth information
  console.log('Birth Date:', insz.birthDate); // Date object
  console.log('Birth Year:', insz.birthYear); // 1985
  
  // Check sex
  console.log('Sex:', insz.sex); // Sex.Male or Sex.Female or Sex.Unknown
  if (insz.sex === Sex.Male) {
    console.log('This is a male');
  }
  
  // Check if it's a BIS number
  console.log('Is BIS:', insz.isBis); // false (regular RRN)
  
  // Get formatted string
  console.log('Formatted:', insz.toFormattedString()); // "85.07.30-033.28"
  console.log('String:', insz.toString()); // "85073003328"
}
```

### Working with BIS Numbers

```typescript
import { InszValidator } from 'johnverbiest.belgian.insz';

// BIS numbers have month increased by 20 or 40
const result = InszValidator.Validate('85273003307'); // Month 27 = 07 + 20

if (result.isValid && result.inszNumber) {
  console.log('Is BIS:', result.inszNumber.isBis); // true
  console.log('Birth Date:', result.inszNumber.birthDate); // July 30, 1985
}
```

### Error Handling

```typescript
import { InszValidator, ValidationError } from 'johnverbiest.belgian.insz';

const result = InszValidator.Validate('12345678901');

if (!result.isValid) {
  console.log('Validation failed!');
  
  result.validationErrors.forEach(error => {
    switch (error) {
      case ValidationError.InputIsNotANumber:
        console.log('Input contains non-numeric characters');
        break;
      case ValidationError.InputIsWrongLength:
        console.log('Input must be exactly 11 digits');
        break;
      case ValidationError.ChecksumIsInvalid:
        console.log('Checksum validation failed');
        break;
      case ValidationError.DateIsInvalid:
        console.log('Birth date is invalid');
        break;
      case ValidationError.InvalidSequenceNumber:
        console.log('Sequence number is out of range');
        break;
    }
  });
}
```

### Working with InszNumber Objects

```typescript
import { InszNumber, InszValidator } from 'johnverbiest.belgian.insz';

// Create an InszNumber object
const insz = new InszNumber('85073003328');

// Validate it
const result = InszValidator.Validate(insz);

// Format the number
console.log(insz.toString()); // "85073003328"
console.log(insz.toFormattedString()); // "85.07.30-033.28"

// Access the numeric value
console.log(insz.value); // 85073003328
```

## 🔧 API Reference

### InszValidator

The main validator class for INSZ/NISS numbers.

#### Methods

##### `Validate(insz: string | number | InszNumber): InszValidationResult`

Validates an INSZ number and returns detailed validation results.

**Parameters:**
- `insz` - The INSZ number to validate (string, number, or InszNumber object)

**Returns:** `InszValidationResult` object containing:
- `isValid: boolean` - Whether the number is valid
- `inszNumber?: InszNumber` - Parsed INSZ object (if valid or partially valid)
- `validationErrors: ValidationError[]` - Array of validation errors

##### Static `Validate(insz: string | number | InszNumber): InszValidationResult`

Static version of the validate method.

### InszNumber

Represents a Belgian INSZ number with extracted information.

#### Properties

- `value: number` - The numeric value of the INSZ
- `hasBeenValidated: boolean` - Whether this number has been validated
- `isValid: boolean | null` - Validation result
- `isBis: boolean | null` - Whether this is a BIS number
- `birthDate: Date | null` - Extracted birth date
- `birthYear: number | null` - Extracted birth year
- `sex: Sex | null` - Extracted sex

#### Methods

- `toString(): string` - Returns the 11-digit string representation
- `toFormattedString(): string | null` - Returns formatted string (XX.XX.XX-XXX.XX)

### Sex Enum

```typescript
enum Sex {
  Unknown = 'Unknown',
  Female = 'Female',
  Male = 'Male'
}
```

### ValidationError Enum

```typescript
enum ValidationError {
  InputIsNotANumber = 'InputIsNotANumber',
  ChecksumIsInvalid = 'ChecksumIsInvalid',
  InputIsWrongLength = 'InputIsWrongLength',
  DateIsInvalid = 'DateIsInvalid',
  InvalidSequenceNumber = 'InvalidSequenceNumber'
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

## 📚 Additional Resources

- [Official Belgian Government Documentation](https://www.ibz.rrn.fgov.be/)
- [INSZ Format Specification](https://www.ksz-bcss.fgov.be/nl/insz)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](../../LICENSE) file for details.

## 👨‍💻 Author

**John Verbiest**

## 🐛 Issues

Found a bug or have a feature request? Please open an issue on [GitHub](https://github.com/johnverbiest/johnverbiest.belgian.insz/issues).

## 🔗 Related Packages

- [.NET Version](https://www.nuget.org/packages/johnverbiest.belgian.insz/) - NuGet package for .NET applications

---

Made with ❤️ in Belgium

