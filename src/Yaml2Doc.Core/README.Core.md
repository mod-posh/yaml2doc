# Yaml2Doc.Core

**Yaml2Doc.Core** is the core library behind Yaml2Doc. It provides:

- A neutral in-memory model (`PipelineDocument`) for generic YAML.
- YAML dialect implementations that interpret different kinds of pipeline YAML:
  - `StandardYamlDialect` � treats input as plain YAML (no CI/CD semantics).
  - `GitHubActionsDialect` � understands GitHub Actions workflows.
  - `AzurePipelinesDialect` � understands Azure DevOps pipeline YAML.
- Loading and parsing helpers (`YamlDocumentContext`, `YamlLoader`).
- A dialect registry (`Yaml2DocRegistry`) for selecting how YAML should be interpreted.

Use this package if you want to **integrate Yaml2Doc�s core parsing and modeling** into your own tools or renderers.

---

## Installation

```bash
dotnet add package Yaml2Doc.Core
````

---

## Basic usage (standard YAML)

The typical flow for plain �standard� YAML is:

1. Load a YAML document into a `YamlDocumentContext`.
2. Use the dialect registry to resolve the Standard YAML dialect.
3. Parse into a `PipelineDocument`.

```csharp
using System.IO;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Engine;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

// Load YAML text
var yamlText = File.ReadAllText("pipeline.yml");

// Create a document context
var context = YamlDocumentContext.FromString(yamlText);

// Set up loader and dialects
var loader = new YamlLoader();
var standardDialect = new StandardYamlDialect(loader);

// For v1.1.0+, you can also register other dialects:
var ghaDialect = new GitHubActionsDialect(loader);
var adoDialect = new AzurePipelinesDialect(loader);

// Register dialect(s)
var registry = new Yaml2DocRegistry(new[] { standardDialect, ghaDialect, adoDialect });

// Resolve the dialect for this document (standard will win by default)
var dialect = registry.ResolveDialect(context);

// Parse into the neutral model
PipelineDocument document = dialect.Parse(context);

// You now have a PipelineDocument with Name + Root keys (and possibly dialect-specific metadata)
```

`PipelineDocument` is intentionally minimal at the core: it exposes a document `Name` (if present) and a `Root` dictionary representing the top-level YAML mapping, plus any extra fields the dialect populates (e.g., triggers/jobs metadata for CI/CD dialects).

---

## Forcing a specific dialect

If you know in advance which dialect you want (e.g., this file is definitely a GitHub Actions workflow), you can use a forced ID when resolving:

```csharp
var registry = new Yaml2DocRegistry(new[] { standardDialect, ghaDialect, adoDialect });

// Ask explicitly for the "gha" dialect
var dialect = registry.ResolveDialect(context, forcedId: "gha");

PipelineDocument document = dialect.Parse(context);
```

The semantic IDs typically match the CLI:

* `standard`
* `gha`
* `ado`

If no `forcedId` is provided, the registry falls back to dialect detection, with the Standard dialect acting as the catch-all for generic YAML documents.

---

## API documentation

For full type and member details, see:

* [Yaml2Doc.Core API documentation](docs/index.md)
