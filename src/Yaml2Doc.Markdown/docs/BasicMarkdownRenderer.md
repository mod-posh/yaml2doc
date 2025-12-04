# BasicMarkdownRenderer

Baseline Markdown renderer that produces a simple overview of a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Remarks**

Renders: - An H1 title using the document name when present; otherwise, the title is "YAML Document". - A "Root Keys" section listing top-level keys. For GitHub Actions (`gha`) and Azure Pipelines (`ado`) dialects, additional best-effort sections are appended while preserving the original baseline output. Output is deterministic for a given document and the input is not mutated.

<a id="yaml2doc.markdown.basicmarkdownrenderer.#ctor"></a>
## Method: #ctor
Initializes a new [BasicMarkdownRenderer](BasicMarkdownRenderer.md) using [Basic](MarkdownRenderMode.md#yaml2doc.markdown.markdownrendermode.basic).

<a id="yaml2doc.markdown.basicmarkdownrenderer.#ctor(yaml2doc.markdown.markdownrendermode)"></a>
## Method: #ctor(MarkdownRenderMode)
Initializes a new [BasicMarkdownRenderer](BasicMarkdownRenderer.md) with the specified `mode`.

**Parameters**
- `mode` — The rendering mode to use for output.

<a id="yaml2doc.markdown.basicmarkdownrenderer.appendazurepipelinessections(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>
## Method: AppendAzurePipelinesSections(PipelineDocument, StringBuilder)
Appends Azure Pipelines-specific sections (Trigger and Stages/Jobs) to the buffer.

**Parameters**
- `document` — The document assumed to represent an Azure DevOps pipeline.
- `sb` — The destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.appendgithubactionssections(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>
## Method: AppendGitHubActionsSections(PipelineDocument, StringBuilder)
Appends GitHub Actions-specific sections (Triggers and Jobs) to the buffer.

**Parameters**
- `document` — The document assumed to represent a GitHub Actions workflow.
- `sb` — The destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.mode"></a>
## Property: Mode
Gets the render mode used by this renderer instance.

<a id="yaml2doc.markdown.basicmarkdownrenderer.render(yaml2doc.core.models.pipelinedocument)"></a>
## Method: Render(PipelineDocument)
Renders the provided `document` to Markdown using the configured [Mode](BasicMarkdownRenderer.md#yaml2doc.markdown.basicmarkdownrenderer.mode).

**Parameters**
- `document` — The [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) to render. Must not be.

**Returns**

Markdown output compliant with the selected mode. In [Basic](MarkdownRenderMode.md#yaml2doc.markdown.markdownrendermode.basic), the output matches the baseline contract.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `document` is.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderazurepipelinestrigger(object,system.text.stringbuilder)"></a>
## Method: RenderAzurePipelinesTrigger(object, StringBuilder)
Renders Azure Pipelines trigger configuration from the value of the `trigger` key.

**Parameters**
- `triggerValue` — The typed value of the `trigger` key.
- `sb` — The destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderbaseline(yaml2doc.core.models.pipelinedocument)"></a>
## Method: RenderBaseline(PipelineDocument)
Baseline representation for standard or unknown dialects: title and "Root Keys" section.

**Parameters**
- `document` — The document to render.

**Returns**

Baseline Markdown without dialect-specific sections.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderbaselineinto(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>
## Method: RenderBaselineInto(PipelineDocument, StringBuilder)
Writes the baseline representation into an existing [StringBuilder](System.Text.StringBuilder.md). The output must be identical to [RenderBaseline(PipelineDocument)](BasicMarkdownRenderer.md#yaml2doc.markdown.basicmarkdownrenderer.renderbaseline(yaml2doc.core.models.pipelinedocument)).

**Parameters**
- `document` — The document to render.
- `sb` — The destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.rendercore(yaml2doc.core.models.pipelinedocument)"></a>
## Method: RenderCore(PipelineDocument)
Converts the provided `document` into the baseline Markdown representation, optionally appending dialect-aware sections for known dialects.

**Parameters**
- `document` — The [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) to render. Must not be.

**Returns**

A Markdown string containing the title and a bullet list of root keys. If there are no root keys, the list is replaced with `_(no root keys)_`. For known dialects (`gha` or `ado`), additional sections are appended.

<a id="yaml2doc.markdown.basicmarkdownrenderer.rendergithubactionstriggers(object,system.text.stringbuilder)"></a>
## Method: RenderGitHubActionsTriggers(object, StringBuilder)
Renders GitHub Actions trigger configuration from the value of the `on` key.

**Parameters**
- `onValue` — The typed value of the `on` key.
- `sb` — The destination buffer.

