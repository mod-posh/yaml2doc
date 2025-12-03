| Latest Version | Nuget.org | Issues | Testing | License | Discord |
|-----------------|-----------------|----------------|----------------|----------------|----------------|
| [![Latest Version](https://img.shields.io/github/v/tag/mod-posh/Yaml2Doc)](https://github.com/mod-posh/yaml2doc/tags) | [![Nuget.org](https://img.shields.io/nuget/dt/ ?label= )](https://www.nuget.org/packages/ ) | [![GitHub issues](https://img.shields.io/github/issues/mod-posh/Yaml2Doc)](https://github.com/mod-posh/yaml2doc/issues) | [![Merge Test Workflow](https://github.com/mod-posh/yaml2doc/actions/workflows/test.yml/badge.svg)](https://github.com/mod-posh/yaml2doc/actions/workflows/test.yml) | [![GitHub license](https://img.shields.io/github/license/mod-posh/Yaml2Doc)](https://github.com/mod-posh/yaml2doc/blob/master/LICENSE) | [![Discord Server](https://assets-global.website-files.com/6257adef93867e50d84d30e2/636e0b5493894cf60b300587_full_logo_white_RGB.svg)](https://discord.com/channels/1044305359021555793/1044305781627035811) |
# Yaml2Doc

Yaml2Doc is a small command-line tool that converts generic YAML into Markdown.

For v1, the focus is intentionally narrow:

- Parse **standard YAML** (no CI/CD semantics required).
- Map it into a neutral in-memory model.
- Emit a simple, predictable Markdown document.

Support for GitHub Actions, Azure DevOps, Jenkins, and other pipeline-specific dialects is explicitly **out of scope for v1** (see [Roadmap](#roadmap--future-work)).

---

## Documentation

API reference is generated from XML docs on each release.

- [Yaml2Doc.Core API Docs](Docs/Yaml2Doc.Core/index.md)
- [Yaml2Doc.Markdown API Docs](Docs/Yaml2Doc.Markdown/index.md)
- [Yaml2Doc.Cli API Docs](Docs/Yaml2Doc.Cli/index.md)

---

## What Yaml2Doc v1 does

Yaml2Doc v1 provides:

- A **Standard YAML dialect**:
  - Treats the input as generic YAML with a mapping at the root.
  - Loads content into a neutral `PipelineDocument` model.
- A **basic Markdown renderer**:
  - `# <Name>` heading based on the `name` field at the root (if present), otherwise `# YAML Document`.
  - A `## Root Keys` section listing the top-level keys in the document.
- A **CLI wrapper** with safe file handling:
  - Resolves user-supplied paths relative to the current working directory.
  - Rejects UNC/device paths and paths that resolve outside the working directory.
  - Blocks traversal through reparse points (symlinks/junctions) inside the working tree.
  - Prevents accidental overwrites by requiring the output file to **not** already exist.

The goal is to have a solid, tested foundation (loader + dialect + renderer + CLI) before layering on any dialect-specific intelligence.

---

## What Yaml2Doc v1 does *not* do

Yaml2Doc v1 does **not**:

- Understand GitHub Actions, Azure DevOps, Jenkins, or any other CI/CD platform semantics.
- Validate that the YAML is a “valid pipeline” for any particular system.
- Render platform-specific sections differently based on `kind`, `apiVersion`, `jobs`, `steps`, etc.
- Act as a linter or schema validator.

Right now, *all* YAML is treated as generic “standard” YAML. Dialects and DSL-aware behavior are planned for later milestones.

---

## Project structure

The solution is split into a few projects:

- `Yaml2Doc.Core`  
  Core types: YAML loading, `PipelineDocument`, dialect abstraction/registry, and the Standard YAML dialect.

- `Yaml2Doc.Markdown`  
  Markdown rendering for `PipelineDocument` (currently the baseline `BasicMarkdownRenderer`).

- `Yaml2Doc.Cli`  
  Console application that wires YAML parsing, dialect selection, and Markdown rendering together behind a simple CLI.

- `Yaml2Doc.Core.Tests`  
  xUnit tests for the loader, dialect registry, renderer, and CLI behavior.

---

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/) (or later compatible SDK).

You can verify your SDK with:

```bash
dotnet --version
````

---

## Building the project

From the repository root:

```bash
dotnet build
```

To run the test suite:

```bash
dotnet test
```

Both commands should succeed before you rely on the tool.

---

## Running Yaml2Doc from source

The simplest way to run the CLI is via `dotnet run` against the CLI project.

### Basic usage

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- <input.yml>
```

Notes:

* The `--` separates `dotnet run` arguments from the CLI’s arguments.
* `<input.yml>` must be a path inside (or below) your current working directory.
* Paths are resolved safely; attempts to escape the working directory or use unsupported path types will be rejected with a clear error message.

The resulting Markdown is written to **standard output**. For example:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml
```

…will print Markdown for the sample “golden” YAML to the console.

You can redirect that to a file:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml > standard-golden.out.md
```

### Writing directly to a file

Yaml2Doc also supports writing output to a specific file.

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- <input.yml> --output <output.md>
```

Example:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml --output out/standard-golden.md
```

Rules:

* `<output.md>` must not already exist. This is to prevent accidental overwrites.
* The output path is also checked to ensure it stays within the working directory and doesn’t traverse via symlinks/junctions.

If the input file is missing, can’t be read, or fails validation, the CLI returns a non-zero exit code and prints an error message to **standard error**.

---

## Example: From clone to Markdown

1. **Clone the repository**

   ```bash
   git clone <your-repo-url> yaml2doc
   cd yaml2doc
   ```

2. **Build and test**

   ```bash
   dotnet build
   dotnet test
   ```

3. **Run against the standard sample**

   ```bash
   dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml
   ```

   You should see Markdown output similar to the expected `samples/pipelines/standard-golden.md`.

4. **Write Markdown to a file**

   ```bash
   mkdir -p out
   dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml --output out/standard-golden.md
   ```

   Open `out/standard-golden.md` in your editor to confirm the output.

---

## Roadmap & future work

Yaml2Doc is designed with pluggable YAML dialects in mind.

Planned future milestones include:

* **Dialect-aware parsing** for:

  * GitHub Actions (`.github/workflows/*.yml`)
  * Azure DevOps pipelines (`azure-pipelines.yml`)
  * Other CI/CD and YAML-based DSLs
* **Dialect selection** via CLI flags (e.g. `--gha`, `--ado`) and/or auto-detection.
* **Richer Markdown output**:

  * Sections that understand jobs/steps, triggers, inputs, etc.
  * Comparison views showing “standard YAML” vs. dialect-specific additions.

For v1, though, the contract is intentionally simple: *standard YAML in, straightforward Markdown out*, with safe file handling and a set of tests to keep behavior predictable.
