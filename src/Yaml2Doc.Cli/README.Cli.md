# Yaml2Doc.Cli

**Yaml2Doc.Cli** is the command-line frontend for Yaml2Doc. It wires together:

- YAML loading and dialect selection from **Yaml2Doc.Core**.
- Markdown rendering from **Yaml2Doc.Markdown**.
- A small CLI host that handles:
  - Safe path resolution
  - Input/output file handling
  - Exit codes and error reporting

Use this package if you are interested in the CLI host behavior, or if you plan to install/run the Yaml2Doc CLI via .NET tooling.

---

## Running the CLI (from source)

From the repository root:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- <input.yml>
````

This will:

* Read `<input.yml>` (which must be inside or below the current working directory).
* Parse it using the **Standard** YAML dialect by default.
* Render a basic Markdown document to **standard output**.

Example:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml
```

You can redirect the output to a file:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml > standard-golden.out.md
```

---

## Dialects and CLI flags

Starting with **v1.1.0**, the CLI supports pluggable YAML dialects:

* `standard` – Generic YAML (default, v1-compatible behavior).
* `gha` – GitHub Actions workflows (`.github/workflows/*.yml`).
* `ado` – Azure DevOps pipelines (`azure-pipelines.yml`, multi-stage YAML, etc.).

### Selecting a dialect via CLI

You can select a dialect explicitly:

```bash
Yaml2Doc --dialect <id> <input.yml>
```

Where `<id>` is one of:

* `standard`
* `gha`
* `ado`

Example:

```bash
Yaml2Doc --dialect gha samples/pipelines/github-golden.yml
```

If you’re running via `dotnet run`, remember to pass arguments after `--`:

```bash
# Standard dialect (implicit, v1-compatible)
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- samples/pipelines/standard-golden.yml

# Explicit GitHub Actions dialect
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- --dialect gha samples/pipelines/github-golden.yml

# Explicit Azure Pipelines dialect
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- --dialect ado samples/pipelines/azure-golden.yml
```

If you **omit** the `--dialect` flag, the CLI behaves exactly like v1.0.0: it uses the Standard dialect and produces the same Markdown for a given standard YAML file.

---

## Writing directly to a file

The CLI also supports an explicit `--output` parameter:

```bash
dotnet run --project src/Yaml2Doc.Cli/Yaml2Doc.Cli.csproj -- <input.yml> --output <output.md>
```

Rules:

* `<output.md>` must **not** already exist (to avoid accidental overwrites).
* The output path must stay within the current working directory (no escaping via symlinks/junctions).

On errors (missing input, invalid path, parse failures, etc.) the CLI:

* Prints a descriptive message to **standard error**.
* Exits with a non-zero exit code.

---

## API documentation

For full type and member details (including the CLI entry point and helpers), see:

* [Yaml2Doc.Cli API documentation](docs/index.md)
