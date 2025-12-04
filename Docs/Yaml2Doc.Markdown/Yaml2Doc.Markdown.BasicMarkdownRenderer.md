# BasicMarkdownRenderer

Baseline Markdown renderer that produces a simple overview of a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Remarks**

Renders: - H1 as the document name if present; otherwise, the title is "YAML Document". - A "Root Keys" section listing top-level keys. For GitHub Actions (`gha`) and Azure Pipelines (`ado`) dialects, additional best-effort sections are appended while preserving the original baseline output. Output is deterministic for a given document and the input is not mutated.

<a id="yaml2doc.markdown.basicmarkdownrenderer.appendazurepipelinessections(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>

## Method: AppendAzurePipelinesSections(PipelineDocument, StringBuilder)

Appends Azure Pipelines-specific sections (Trigger, Stages/Jobs) to the buffer.

**Parameters**

- `document` — The document assumed to represent an Azure DevOps pipeline.
- `sb` — Destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.appendgithubactionssections(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>

## Method: AppendGitHubActionsSections(PipelineDocument, StringBuilder)

Appends GitHub Actions-specific sections (Triggers, Jobs) to the buffer.

**Parameters**

- `document` — The document assumed to represent a GitHub Actions workflow.
- `sb` — Destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.render(yaml2doc.core.models.pipelinedocument)"></a>

## Method: Render(PipelineDocument)

Converts the provided `document` into a basic Markdown representation.

**Parameters**

- `document` — The [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) to render. Must not be.

**Returns**

A Markdown string containing the document title and a bullet list of root keys. If there are no root keys, the list is replaced with `_(no root keys)_`. For known dialects (`gha` or `ado`), additional sections are appended.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `document` is.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderazurepipelinestrigger(object,system.text.stringbuilder)"></a>

## Method: RenderAzurePipelinesTrigger(object, StringBuilder)

Renders Azure Pipelines trigger configuration from the value of the `trigger` key.

**Parameters**

- `triggerValue` — The typed value of the `trigger` key.
- `sb` — Destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderbaseline(yaml2doc.core.models.pipelinedocument)"></a>

## Method: RenderBaseline(PipelineDocument)

Baseline representation: title and "Root Keys" section (used for standard/unknown dialects).

**Parameters**

- `document` — The document to render.

**Returns**

Baseline Markdown without dialect-specific sections.

<a id="yaml2doc.markdown.basicmarkdownrenderer.renderbaselineinto(yaml2doc.core.models.pipelinedocument,system.text.stringbuilder)"></a>

## Method: RenderBaselineInto(PipelineDocument, StringBuilder)

Writes the baseline representation into an existing [StringBuilder](System.Text.StringBuilder.md). Must preserve identical textual output as [RenderBaseline(PipelineDocument)](Yaml2Doc.Markdown.BasicMarkdownRenderer.md#yaml2doc.markdown.basicmarkdownrenderer.renderbaseline(yaml2doc.core.models.pipelinedocument)).

**Parameters**

- `document` — The document to render.
- `sb` — Destination buffer.

<a id="yaml2doc.markdown.basicmarkdownrenderer.rendergithubactionstriggers(object,system.text.stringbuilder)"></a>

## Method: RenderGitHubActionsTriggers(object, StringBuilder)

Renders GitHub Actions trigger configuration from the value of the `on` key.

**Parameters**

- `onValue` — The typed value of the `on` key.
- `sb` — Destination buffer.
