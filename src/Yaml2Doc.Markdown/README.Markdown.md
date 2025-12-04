# Yaml2Doc.Markdown

**Yaml2Doc.Markdown** provides a baseline Markdown renderer for the Yaml2Doc core model (`PipelineDocument`).

For v1.x, the renderer is intentionally small and predictable:

- Renders a top-level heading from the document `Name`, if present.
- Falls back to `# YAML Document` if no `name` is provided.
- Emits a `## Root Keys` section listing the top-level keys in the YAML document.

Starting with **v1.1.0**, the renderer can also surface **dialect-aware sections** when the `PipelineDocument` includes richer, dialect-specific metadata (e.g., triggers, jobs, steps). For a plain “standard YAML” document, the output remains identical to v1.0.0.

Use this package if you already have a `PipelineDocument` and just want a **predictable Markdown representation**, with optional dialect-aware extras when available.

---

## Installation

```bash
dotnet add package Yaml2Doc.Markdown
````

---

## Basic usage

Given a `PipelineDocument` (usually produced by `Yaml2Doc.Core`):

```csharp
using Yaml2Doc.Core.Models;
using Yaml2Doc.Markdown;

// Suppose you obtained this from Yaml2Doc.Core
PipelineDocument document = GetPipelineDocumentSomehow();

// Create the renderer
IMarkdownRenderer renderer = new BasicMarkdownRenderer();

// Render to Markdown
string markdown = renderer.Render(document);

// Write or use the Markdown
Console.WriteLine(markdown);
```

Typical output shape for standard YAML:

```md
# My Pipeline

## Root Keys

- name
- trigger
- jobs
```

For documents produced by dialects (e.g., GitHub Actions / Azure Pipelines), the renderer may additionally emit sections such as:

```md
## Trigger
...

## Jobs
...

## Steps
...
```

These sections are **additive**: they sit alongside the existing heading and `Root Keys` section.

You can implement your own `IMarkdownRenderer` if you need a different layout or more opinionated dialect-specific Markdown.

---

## API documentation

For full type and member details, see:

* [Yaml2Doc.Markdown API documentation](docs/index.md)
