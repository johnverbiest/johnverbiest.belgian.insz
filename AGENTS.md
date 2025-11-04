# AGENTS.md

> 📦 **Project goal:** Provide the same Belgian INSZ / Rijksregisternummer / BIS validation logic as two SDKs:
> - a **.NET NuGet package** (`Be.Identifiers.NationalNumber`)
> - an **npm package** (`@be-identifiers/national-number`)
>
> Both implementations must conform to the normative rules in **`docs/BelgiumRegistryRules.md`** and be verified against the **shared conformance vectors**.

---

## 🧠 Agent Overview

| Agent | Role | Language | Output | Trigger |
|:------|:------|:----------|:--------|:---------|
| `lib-builder` | Builds multi-target library | C# (.NET 8, 6, .NET Standard 2.0) | `bin/Debug/johnverbiest.belgian.insz.dll` (multi-target) | On commit |
| `test-shared` | Shared test suite (Fact definitions) | C# (.NET Standard 2.0) | `src/dotnet/johnverbiest.belgian.insz.tests/` | Updated when test logic changes |
| `test-modern-runner` | Runs tests on modern .NET versions | C# (net6.0, net8.0, net481) | xunit test results | `dotnet test johnverbiest.belgian.insz.tests.modern` |
| `test-framework481-runner` | Runs tests on .NET Framework 4.8.1 | C# (.NET Framework 4.8.1) | xunit test results | Via legacy runner |
| `publisher` | Publishes NuGet package when all tests pass | — | `Be.Identifiers.NationalNumber.*.nupkg` | On `v*` git tag |

---

## 📁 Repository Layout

```
NationalNumber.Be.DotNet/
├── AGENTS.md                             # This file
├── README.md
├── src/
│   └── dotnet/
│       ├── johnverbiest.belgian.insz.sln
│       ├── johnverbiest.belgian.insz/
│       │   ├── johnverbiest.belgian.insz.csproj    # Multi-target lib (net8.0, net6.0, netstandard2.0)
│       │   └── <All the code files>
│       ├── johnverbiest.belgian.insz.tests/         # Shared test library (netstandard2.0)
│       │   ├── johnverbiest.belgian.insz.tests.csproj
│       │   └── SharedTests/
│       │       └── <Test suites>
│       ├── johnverbiest.belgian.insz.tests.modern/  # Test runner (net6.0, net8.0, net481)
│       │   ├── johnverbiest.belgian.insz.tests.modern.csproj
│       │   └── Links to SharedTests via <Compile Include>
│       ├── johnverbiest.belgian.insz.tests.framework481/  # Legacy .NET Framework 4.8.1 runner
│       │   ├── johnverbiest.belgian.insz.tests.framework481.csproj
│       │   └── Links to SharedTests via <Compile Include>
│       ├── packages/                     # NuGet packages for legacy runner
│       └── README.md
└── .github/
    └── workflows/
        └── (CI/CD workflows if present)
```

---

## 🧩 Agent Specifications

### **lib-builder**
- Builds the main library (`johnverbiest.belgian.insz`).
- **Target Frameworks:**
  - `.NET 8.0`
  - `.NET 6.0`
  - `.NET Standard 2.0` (for broad compatibility)
- **Configuration:**
  - Implicit usings enabled
  - Nullable reference types enabled
  - Latest C# language version
- **Artifact:** Multi-target DLL with platform-specific optimizations.

### **test-shared**
- **Location:** `src/dotnet/johnverbiest.belgian.insz.tests/`
- **Language:** C# (.NET Standard 2.0)
- **Role:** Contains all test logic (`[Fact]` methods, test utilities).
- **Configuration:**
  - LangVersion: 9.0
  - xunit 2.4.2
- **How it works:** Other test runners link these tests via `<Compile Include>` without code duplication.

### **test-modern-runner**
- **Location:** `src/dotnet/johnverbiest.belgian.insz.tests.modern/`
- **Target Frameworks:** `net6.0`, `net8.0`, `net481`
- **Framework:** xunit 2.5.3 with Microsoft.NET.Test.Sdk 17.10.0
- **Test Execution:**
  ```bash
  dotnet test src/dotnet/johnverbiest.belgian.insz.tests.modern
  ```
- **Linking:** Tests linked from `johnverbiest.belgian.insz.tests/SharedTests/**/*.cs` via project file `<Compile Include>`.

### **test-framework481-runner**
- **Location:** `src/dotnet/johnverbiest.belgian.insz.tests.framework481/`
- **Target Framework:** `.NET Framework 4.8.1` (legacy)
- **Package References:** xunit 2.1.0 (via `packages.config`)
- **Dependencies:** Resolved from `packages/` folder.
- **Test Execution:**
  ```bash
  cd src/dotnet
  msbuild johnverbiest.belgian.insz.tests.framework481/johnverbiest.belgian.insz.tests.framework481.csproj /t:Build /t:Test
  ```
- **Note:** Provides backward compatibility for legacy .NET Framework users.

### **publisher**
- **Trigger:** Git tag matching `v*` (e.g., `v1.0.0`).
- **Steps:**
  1. Run `dotnet test src/dotnet` (all runners execute).
  2. If all tests pass:
     - Build: `dotnet pack src/dotnet/johnverbiest.belgian.insz/johnverbiest.belgian.insz.csproj`
     - Publish: `dotnet nuget push **/*.nupkg --api-key $NUGET_KEY --source https://api.nuget.org/v3/index.json`
  3. Create GitHub Release with package metadata and links.

---