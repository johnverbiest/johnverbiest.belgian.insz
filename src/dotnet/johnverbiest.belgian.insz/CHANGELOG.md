# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned for 1.0.0
- NPM package for JavaScript/TypeScript support

## [0.1.0] - 2025-11-18

### Added
- Initial release of Belgian INSZ/NISS Number Validator
- Full validation support for INSZ/NISS numbers
- BIS number detection and validation
- Birth date extraction with century detection
- Sex determination from sequence numbers (where applicable)
- Support for multiple input formats (string, long, formatted)
- Static methods for simple usage scenarios
- Instance methods implementing IInszValidator for dependency injection
- Comprehensive XML documentation
- Support for .NET 8.0, .NET 6.0, and .NET Standard 2.0
- Thread-safe validation with lazy-loaded properties
- Detailed validation error reporting
- Zero external dependencies (except SourceLink for debugging)

### Features
- Validates checksum using modulo 97 algorithm
- Handles pre-2000 and post-2000 birth dates automatically
- Supports BIS numbers with unknown sex (+20) and known sex (+40)
- Automatic formatting character stripping (dots, dashes, spaces)
- Formatted output (YY.MM.DD-NNN.CC)

[Unreleased]: https://github.com/johnverbiest/NationalNumber.Be.DotNet/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/johnverbiest/NationalNumber.Be.DotNet/releases/tag/v0.1.0

