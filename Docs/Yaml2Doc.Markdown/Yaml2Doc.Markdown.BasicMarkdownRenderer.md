# BasicMarkdownRenderer

Baseline Markdown renderer that produces a simple overview of a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Remarks**

Renders: - H1 as the document name if present; otherwise, the title is "YAML Document". - A "Root Keys" section listing top-level keys. The renderer does not mutate the input [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) and produces deterministic output for a given input.

<a id="yaml2doc.markdown.basicmarkdownrenderer.render(yaml2doc.core.models.pipelinedocument)"></a>

## Method: Render(PipelineDocument)

Converts the provided `document` into a basic Markdown representation.

**Parameters**

- `document` — The [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) to render. Must not be.

**Returns**

A Markdown string containing the document title and a bullet list of root keys. If there are no root keys, the list is replaced with `_(no root keys)_`.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `document` is.
