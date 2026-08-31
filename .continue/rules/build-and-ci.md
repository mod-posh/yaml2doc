---
name: Yaml2Doc build and CI
globs:
  [
    "**/*.cs",
    "**/*.csproj",
    "*.sln",
    "*.props",
    ".github/workflows/*.{yml,yaml}",
  ]
alwaysApply: false
description: Verified local and CI commands for the Yaml2Doc solution
---

# Build And CI

Run day-to-day commands from the repository root:

```powershell
dotnet build
dotnet test
```

The build and pull-request workflows use the explicit Release sequence:

```powershell
dotnet clean Yaml2Doc.sln --configuration Release
dotnet restore Yaml2Doc.sln
dotnet build Yaml2Doc.sln --configuration Release --no-restore
dotnet test Yaml2Doc.sln --configuration Release --no-build --logger "trx;LogFileName=test_results.trx"
```

- `.github/workflows/build.yml` and `.github/workflows/test.yml` run on `windows-latest`.
- `.github/workflows/release.yml` runs on `ubuntu-latest` and installs PowerShell before its `pwsh` steps.
- Packaging is performed per project by the build/release workflows; do not invent or substitute repository scripts.
- Release API documentation is generated into `Docs/`. Follow [agent-workflow.md](agent-workflow.md) rather than editing generated output.
