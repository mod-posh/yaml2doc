# Yaml2Doc



Yaml2Doc is a small command-line tool that converts generic YAML into Markdown.



For v1, the focus is intentionally narrow:



* Parse **standard YAML** (no CI/CD semantics required).

* Map it into a neutral in-memory model.

* Emit a simple, predictable Markdown document.



Support for GitHub Actions, Azure DevOps, Jenkins, and other pipeline-specific dialects is explicitly **out of scope for v1** (see [Roadmap](#roadmap--future-work)).



---



## What Yaml2Doc v1 does



Yaml2Doc v1 provides:



* A **Standard YAML dialect**:

&nbsp; * Treats the input as generic YAML with a mapping at the root.

&nbsp; * Loads content into a neutral `PipelineDocument` model.

* A **basic Markdown renderer**:

&nbsp; * `# <Name>` heading based on the `name` field at the root (if present), otherwise `# YAML Document`.

&nbsp; * A `## Root Keys` section listing the top-level keys in the document.

* A **CLI wrapper** with safe file handling:

&nbsp; * Resolves user-supplied paths relative to the current working directory.

&nbsp; * Rejects UNC/device paths and paths that resolve outside the working directory.

&nbsp; * Blocks traversal through reparse points (symlinks/junctions) inside the working tree.

&nbsp; * Prevents accidental overwrites by requiring the output file to **not** already exist.



The goal is to have a solid, tested foundation (loader + dialect + renderer + CLI) before layering on any dialect-specific intelligence.



---



## What Yaml2Doc v1 does *not* do



Yaml2Doc v1 does **not**:



* Understand GitHub Actions, Azure DevOps, Jenkins, or any other CI/CD platform semantics.

* Validate that the YAML is a “valid pipeline” for any particular system.

* Render platform-specific sections differently based on `kind`, `apiVersion`, `jobs`, `steps`, etc.

* Act as a linter or schema validator.



Right now, *all* YAML is treated as generic “standard” YAML. Dialects and DSL-aware behavior are planned for later milestones.



---



## Project structure



The solution is split into a few projects:



* `Yaml2Doc.Core`  

&nbsp; Core types: YAML loading, `PipelineDocument`, dialect abstraction/registry, and the Standard YAML dialect.



* `Yaml2Doc.Markdown`  

&nbsp; Markdown rendering for `PipelineDocument` (currently the baseline `BasicMarkdownRenderer`).



* `Yaml2Doc.Cli`  

&nbsp; Console application that wires YAML parsing, dialect selection, and Markdown rendering together behind a simple CLI.



* `Yaml2Doc.Core.Tests`  

&nbsp; xUnit tests for the loader, dialect registry, renderer, and CLI behavior.



---



## Prerequisites



* [.NET SDK 9.0](https://dotnet.microsoft.com/) (or later compatible SDK).



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



1\. **Clone the repository**



&nbsp;  ```bash

&nbsp;  git clone <your-repo-url> yaml2doc

&nbsp;  cd yaml2doc

&nbsp;  ```



2\. **Build and test**



&nbsp;  ```bash

&nbsp;  dotnet build

&nbsp;  dotnet test

&nbsp;  ```



3\. **Run against the standard sample**



&nbsp;  ```bash

&nbsp;  dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml

&nbsp;  ```



&nbsp;  You should see Markdown output similar to the expected `samples/pipelines/standard-golden.md`.



4\. **Write Markdown to a file**



&nbsp;  ```bash

&nbsp;  mkdir -p out

&nbsp;  dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml --output out/standard-golden.md

&nbsp;  ```



&nbsp;  Open `out/standard-golden.md` in your editor to confirm the output.



---



## Roadmap \& future work



Yaml2Doc is designed with pluggable YAML dialects in mind.



Planned future milestones include:



* **Dialect-aware parsing** for:



&nbsp; * GitHub Actions (`.github/workflows/*.yml`)

&nbsp; * Azure DevOps pipelines (`azure-pipelines.yml`)

&nbsp; * Other CI/CD and YAML-based DSLs

* **Dialect selection** via CLI flags (e.g. `--gha`, `--ado`) and/or auto-detection.

* **Richer Markdown output**:



&nbsp; * Sections that understand jobs/steps, triggers, inputs, etc.

&nbsp; * Comparison views showing “standard YAML” vs. dialect-specific additions.



For v1, though, the contract is intentionally simple: *standard YAML in, straightforward Markdown out*, with safe file handling and a set of tests to keep behavior predictable.



