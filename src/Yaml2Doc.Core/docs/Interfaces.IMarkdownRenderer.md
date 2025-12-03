# IMarkdownRenderer

Defines a contract for rendering a [PipelineDocument](Models.PipelineDocument.md) into Markdown.

**Remarks**

Implementations should be deterministic for the same input, avoid mutating the provided [PipelineDocument](Models.PipelineDocument.md), and may be called multiple times with different documents.

<a id="yaml2doc.core.interfaces.imarkdownrenderer.render(yaml2doc.core.models.pipelinedocument)"></a>
## Method: Render(PipelineDocument)
Renders the specified `document` into a Markdown string.

**Parameters**
- `document` — The [PipelineDocument](Models.PipelineDocument.md) to render. Must not be.

**Returns**

A [String](System.String.md) containing the Markdown representation of the provided [PipelineDocument](Models.PipelineDocument.md). Never returns.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `document` is.

