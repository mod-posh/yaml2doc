# Yaml2Doc.Markdown

**Yaml2Doc.Markdown** provides a baseline Markdown renderer for the Yaml2Doc core model (`PipelineDocument`).

For v1, the renderer is intentionally simple:

- Renders a top-level heading from the document `Name`, if present.
- Falls back to `# YAML Document` if no `name` is provided.
- Emits a `## Root Keys` section listing the top-level keys in the YAML document.

Use this package if you already have a `PipelineDocument` and just want a **predictable Markdown representation**.

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

Typical output shape:

```md
# My Pipeline

## Root Keys

- name
- trigger
- jobs
```

You can implement your own `IMarkdownRenderer` if you need richer or dialect-specific Markdown.

---

## API documentation

For full type and member details, see:

* [Yaml2Doc.Markdown API documentation](docs/index.md)
