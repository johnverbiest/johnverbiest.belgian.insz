# Shared Test Architecture

This solution demonstrates how to share test code across multiple target frameworks using the "shared tests + runner projects" pattern.

## Project Structure

### 1. `johnverbiest.belgian.insz.tests` (Shared Test Library)
- **Target Framework**: netstandard2.0
- **Purpose**: Contains the actual test code (`NationalNumberTests.cs`)
- **Key Features**:
  - Uses xUnit for test framework
  - References the main library being tested
  - Cannot be executed directly (netstandard2.0 is not executable)
  - Suppresses assembly info generation to avoid conflicts

### 2. `johnverbiest.belgian.insz.tests.framework481` (Framework 4.8.1 Runner)
- **Target Framework**: .NET Framework 4.8.1
- **Purpose**: Test runner for Framework 4.8.1
- **Key Features**:
  - Links shared test files using `&lt;Compile Include="..\johnverbiest.belgian.insz.tests\NationalNumberTests.cs" Link="SharedTests\NationalNumberTests.cs" /&gt;`
  - Uses traditional packages.config for NuGet packages
  - Contains its own AssemblyInfo.cs

### 3. `johnverbiest.belgian.insz.tests.modern` (Multi-Target Modern Runner)
- **Target Frameworks**: net6.0;net8.0
- **Purpose**: Test runner for modern .NET versions
- **Key Features**:
  - Links shared test files 
  - Uses modern SDK-style project format
  - Uses modern test SDK packages
  - Multi-targets both .NET 6 and .NET 8

## Benefits

1. **Single Source of Truth**: Test logic is maintained in one place
2. **Multi-Framework Support**: Same tests run across Framework 4.8.1, .NET 6, and .NET 8
3. **Maintainability**: Changes to tests only need to be made in the shared project
4. **Framework-Specific Runners**: Each runner can use framework-appropriate test packages and configurations

## Test Results

The shared tests successfully run across different frameworks:
- ✅ .NET 8.0: All 6 tests passed
- ✅ .NET Framework 4.8.1: Builds successfully, shares the same test code
- ⚠️ .NET 6.0: Builds successfully (runtime not installed in test environment)

## Example Usage

```bash
# Build all projects
dotnet build johnverbiest.belgian.insz.sln

# Run tests on modern frameworks
dotnet test johnverbiest.belgian.insz.tests.modern

# Run tests on Framework 4.8.1 (requires appropriate test runner)
# The framework481 project compiles and links the shared tests correctly
```

## Alternative: Multi-Target Single Project

For simpler scenarios, you could also use a single test project with multi-targeting:

```xml
&lt;TargetFrameworks&gt;net8.0;net6.0;net48&lt;/TargetFrameworks&gt;
```

However, the shared tests + runners approach provides more flexibility for framework-specific configurations and package dependencies.
