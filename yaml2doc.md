# Yaml2Doc

Yaml2Doc is a small command-line tool that converts YAML into Markdown.

For the **v1.x** series, the focus is intentionally narrow:

- Parse YAML into a neutral in-memory model.
- Emit a simple, predictable Markdown document.
- Keep behavior stable and testable as we add optional features.

Starting with **v1.1.0**, Yaml2Doc adds **pluggable pipeline-aware dialects** for:

- GitHub Actions
- Azure DevOps pipelines

…without changing the default behavior for plain “standard” YAML.

---

## Documentation

API reference is generated from XML docs on each release.

- [Yaml2Doc.Core API Docs](Docs/Yaml2Doc.Core/index.md)
- [Yaml2Doc.Markdown API Docs](Docs/Yaml2Doc.Markdown/index.md)
- [Yaml2Doc.Cli API Docs](Docs/Yaml2Doc.Cli/index.md)

---

## Versioning and current release

> **Current release:** `v1.1.0`

Yaml2Doc follows semantic versioning:

- **v1.0.0** – Baseline: parse standard YAML and emit generic Markdown using the Standard dialect.
- **v1.1.0** – Adds *optional* GitHub Actions (`gha`) and Azure Pipelines (`ado`) dialects, plus a CLI flag to select them.

This is a **non-breaking minor** release because:

- If you run `Yaml2Doc` **without** any dialect flags, behavior is unchanged from v1.0.0.
- Existing “standard YAML” workflows and golden tests still pass byte-for-byte.
- New behavior is opt-in via the `--dialect` flag.

---

## What Yaml2Doc v1.x does

Yaml2Doc v1.x provides:

- A **Standard YAML dialect**:
  - Treats the input as generic YAML with a mapping at the root.
  - Loads content into a neutral `PipelineDocument` model.

- Additional **pipeline-aware dialects** (v1.1.0+):
  - GitHub Actions dialect for `.github/workflows/*.yml`.
  - Azure Pipelines dialect for `azure-pipelines.yml` and similar files.
  - These dialects populate extra metadata in `PipelineDocument` (e.g., triggers, jobs, steps) that the Markdown renderer can surface.

- A **basic Markdown renderer**:
  - `# <Name>` heading based on the `name` field at the root (if present), otherwise `# YAML Document`.
  - A `## Root Keys` section listing the top-level keys in the document.
  - For dialect-aware documents, additional sections (e.g., `## Trigger`, `## Jobs`, `## Steps`) are added on top of the baseline output.

- A **CLI wrapper** with safe file handling:
  - Resolves user-supplied paths relative to the current working directory.
  - Rejects UNC/device paths and paths that resolve outside the working directory.
  - Blocks traversal through reparse points (symlinks/junctions) inside the working tree.
  - Prevents accidental overwrites by requiring the output file to **not** already exist.
  - Supports dialect selection via `--dialect <id>`.

The goal is to have a solid, tested foundation (loader + dialects + renderer + CLI) that still behaves exactly like v1.0.0 for standard YAML, while enabling richer output for CI/CD YAML when you opt in.

---

## What Yaml2Doc v1.x does *not* do

Even with dialects, Yaml2Doc v1.x does **not**:

- Validate that the YAML is a “valid pipeline” for any particular system.
- Act as a linter or schema validator.
- Enforce CI/CD semantics like matrix strategies, reusable workflows, or Azure DevOps templates.
- Replace official tools (e.g., `act`, `azure-pipelines` validator, etc.).

Dialects provide a **friendlier Markdown view** over pipeline YAML, not full semantic validation.

---

## Project structure

The solution is split into a few projects:

- `Yaml2Doc.Core`  
  Core types: YAML loading, `PipelineDocument`, dialect abstraction/registry, and concrete dialects (Standard, GitHub Actions, Azure Pipelines).

- `Yaml2Doc.Markdown`  
  Markdown rendering for `PipelineDocument` (baseline `BasicMarkdownRenderer` plus optional dialect-aware sections).

- `Yaml2Doc.Cli`  
  Console application that wires YAML parsing, dialect selection, and Markdown rendering together behind a simple CLI.

- `Yaml2Doc.Core.Tests`  
  xUnit tests for the loader, dialect registry, renderer, and CLI behavior (including golden tests for standard and dialect-specific Markdown).

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

### Basic usage (standard YAML)

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
mkdir -p out
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml --output out/standard-golden.md
```

Rules:

* `<output.md>` must not already exist. This is to prevent accidental overwrites.
* The output path is also checked to ensure it stays within the working directory and doesn’t traverse via symlinks/junctions.

If the input file is missing, can’t be read, or fails validation, the CLI returns a non-zero exit code and prints an error message to **standard error**.

---

## Dialects and usage examples

### Dialect IDs

Yaml2Doc currently ships with three dialects:

* `standard` – Generic YAML (default).
* `gha` – GitHub Actions workflows.
* `ado` – Azure DevOps pipelines.

### Selecting a dialect via CLI

You can select a dialect explicitly:

```bash
Yaml2Doc --dialect <id> <input.yml>
```

Where `<id>` is one of `standard`, `gha`, or `ado`.

When running via `dotnet run`:

```bash
# Standard dialect (implicit, v1-compatible)
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml

# Explicit GitHub Actions dialect
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- --dialect gha samples/pipelines/github-golden.yml

# Explicit Azure Pipelines dialect
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- --dialect ado samples/pipelines/azure-golden.yml
```

If you omit the dialect flag entirely, Yaml2Doc behaves exactly like v1.0.0 (Standard dialect only).

### Example: GitHub Actions workflow

Assuming you have a sample workflow at `samples/pipelines/github-golden.yml`:

```bash
Yaml2Doc --dialect gha samples/pipelines/github-golden.yml > github-golden.md
```

The generated Markdown will:

* Keep the familiar structure:

  * `# <Name>` heading (e.g., `# CI`)
  * `## Root Keys` section listing `name`, `on`, `jobs`, etc.

* Add **GitHub Actions-aware sections**, for example:

  * `## Triggers` – summarizing `on:` (push, pull_request, branches, etc.).
  * `## Jobs` – high-level job list.
  * `## Steps` – bullet points or tables for key steps per job.

These extra sections are *additive*: they don’t remove or change the original header or `Root Keys` section.

### Example: Azure DevOps pipeline

Assuming you have a sample pipeline at `samples/pipelines/azure-golden.yml`:

```bash
Yaml2Doc --dialect ado samples/pipelines/azure-golden.yml > azure-golden.md
```

The generated Markdown will:

* Preserve the same baseline:

  * `# <Name>` heading (e.g., `# CI`)
  * `## Root Keys` section listing `name`, `trigger`, `pool`, `steps` / `stages`, etc.

* Add **Azure Pipelines-aware sections**, for example:

  * `## Trigger` – summarizing `trigger:` branches and conditions.
  * `## Stages` / `## Jobs` – structured overview of stages and jobs.
  * `## Steps` – key steps for each job.

Again, these dialect-aware sections are layered **on top of** the v1 Markdown. If you switch back to:

```bash
Yaml2Doc azure-pipelines.yml
```

(with no `--dialect` flag), you get the original v1-style output without the extra sections.

---

## Roadmap & future work

Yaml2Doc is built around pluggable YAML dialects.

**Delivered in v1.1.0:**

* Standard / GitHub Actions / Azure Pipelines dialects.
* Dialect selection via `--dialect <id>`.
* Golden tests for standard and pipeline YAML to lock in behavior.

**Planned future milestones:**

* Additional dialects:

  * Jenkins pipelines
  * Other YAML-based DSLs

* Richer Markdown output:

  * More detailed sections for jobs/steps, triggers, inputs, matrices, etc.
  * Comparison views showing “standard YAML” vs. dialect-specific additions.

For now, the contract stays simple: *standard YAML in, straightforward Markdown out* by default, with optional dialect-aware Markdown when you ask for it.
