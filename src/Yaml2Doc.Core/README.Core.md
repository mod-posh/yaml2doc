# Yaml2Doc.Core

**Yaml2Doc.Core** is the core library behind Yaml2Doc. It provides:

- A neutral in-memory model (`PipelineDocument`) for generic YAML.
- A Standard YAML dialect (`StandardYamlDialect`) that treats input as plain YAML (no CI/CD semantics).
- Loading and parsing helpers (`YamlDocumentContext`, `YamlLoader`).
- A simple dialect registry for selecting how YAML should be interpreted.

Use this package if you want to **integrate Yaml2Doc’s core parsing and modeling** into your own tools or renderers.

---

## Installation

```bash
dotnet add package Yaml2Doc.Core
````

---

## Basic usage

The typical flow is:

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

// Set up loader and dialect
var loader = new YamlLoader();
var standardDialect = new StandardYamlDialect(loader);

// Register dialect(s)
var registry = new Yaml2DocRegistry(new[] { standardDialect });

// Resolve the dialect for this document
var dialect = registry.ResolveDialect(context);

// Parse into the neutral model
PipelineDocument document = dialect.Parse(context);

// You now have a PipelineDocument with Name + Root keys
```

`PipelineDocument` is intentionally minimal for v1: it exposes a document `Name` (if present) and a `Root` dictionary representing the top-level YAML mapping.

---

## API documentation

For full type and member details, see:

* [Yaml2Doc.Core API documentation](docs/index.md)
