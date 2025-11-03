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
| `spec-guardian` | Maintains the normative rule spec | Markdown (gov sources only) | `docs/BelgiumRegistryRules.md` | Manual edits / official updates |
| `vector-builder` | Generates and maintains test vectors | JSON | `conformance/vectors/*.json` | After rule change |
| `dotnet-builder` | Builds and tests the .NET package | C# (.NET 8 +) | `nuget/Be.Identifiers.NationalNumber.*.nupkg` | On commit / tag |
| `npm-builder` | Builds and tests the npm package | TypeScript (ES 2022) | `npm/dist/@be-identifiers/national-number` | On commit / tag |
| `cross-tester` | Runs shared test vectors against both SDKs | Node + .NET | CI job | PR / Release candidate |
| `publisher` | Publishes both artifacts when all tests pass | — | NuGet + npm registries | On `v*` tag |

---

## 📁 Repository Layout

```
be-identifiers/
├── docs/
│   └── BelgiumRegistryRules.md           # Normative government-sourced specification
├── conformance/
│   └── vectors/
│       ├── valid.json                    # ✅ Valid test cases
│       ├── invalid.json                  # ❌ Invalid test cases
│       └── edge.json                     # 🧪 Edge cases
├── src/
│   ├── dotnet/
│   │   ├── johnverbiest.belgian.insz.sln
│   │   ├── johnverbiest.belgian.insz/johnverbiest.belgian.insz.csproj
│   │   ├── johnverbiest.belgian.insz.tests/johnverbistest.belgian.insz.tests.csproj
│   └── npm/
│       ├── package.json
│       ├── src/
│       │   └── index.ts
│       └── test/
│           └── vectors.spec.ts
└── .github/
    └── workflows/
        └── ci.yml
```

---

## 🧩 Agent Specifications

### **spec-guardian**
- Watches `docs/BelgiumRegistryRules.md`.
- Validates all external links (404 check) in CI.
- Prevents merging of rule changes without proper source citations.

### **vector-builder**
- Reads the spec, generates canonical examples:
  - valid RN/BIS numbers with correct checksums and parity.
  - invalid cases (wrong checksum, impossible dates, BIS offsets, etc.).
- Writes to `conformance/vectors/`.
- Exposes a helper script:  
  ```bash
  pnpm run generate:vectors
  dotnet run --project tools/VectorBuilder
  ```

### **npm-builder**
- Language: **TypeScript 5 + ES 2022**
- Package name: **`@be-identifiers/national-number`**
- Responsibilities:
  - Same logic as C# version (functional parity).
  - Exports:
    ```ts
    export function isValid(input: string): boolean;
    export function tryParse(input: string): Parsed | null;
    ```
  - Test suite (`Vitest` or `Jest`) loads the same JSON vectors for validation.
  - Bundled via `tsup` / `rollup` → ES module.
- NPM metadata:
  - `author`: "Be Identifiers Project"
  - `license`: "MIT"
  - `repository`: "github:<you>/be-identifiers"
  - `files`: `["dist", "LICENSE", "README.md"]`

### **cross-tester**
- Runs after both test suites pass.
- Uses Node’s `child_process` to call:
  ```bash
  dotnet test src/dotnet
  npm test --prefix src/npm
  ```
- Then compares outputs from both SDKs for every entry in `valid.json` and `invalid.json` to assert parity.

### **publisher**
- Trigger: git tag starting with `v`.
- Steps:
  1. Run `cross-tester`.
  2. If green → publish:
     - `.NET`: `dotnet nuget push **/*.nupkg --api-key $NUGET_KEY`
     - `npm`: `npm publish --access public`
  3. Create GitHub Release with changelog and links to both packages.

---