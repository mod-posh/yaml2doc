# Yaml2DocEngine

Main orchestration entry point for converting YAML text into Markdown.

**Remarks**

The engine: 1) Builds a [YamlDocumentContext](Parsing.YamlDocumentContext.md) from the raw YAML. 2) Resolves a dialect via [Yaml2DocRegistry](Dialects.Yaml2DocRegistry.md) (optionally by identifier). 3) Parses the context into a [PipelineDocument](Models.PipelineDocument.md). 4) Renders the document to Markdown using the configured [IMarkdownRenderer](Interfaces.IMarkdownRenderer.md). Instances are immutable after construction; dependencies are required and validated at initialization.

<a id="yaml2doc.core.engine.yaml2docengine.#ctor(yaml2doc.core.dialects.yaml2docregistry,yaml2doc.core.interfaces.imarkdownrenderer)"></a>
## Method: #ctor(Yaml2DocRegistry, IMarkdownRenderer)
Initializes a new instance of the [Yaml2DocEngine](Engine.Yaml2DocEngine.md) class.

**Parameters**
- `registry` — The registry that resolves dialects for parsing.
- `markdownRenderer` — The renderer used to produce Markdown output.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `registry` or `markdownRenderer` is.

<a id="yaml2doc.core.engine.yaml2docengine.convert(string,string)"></a>
## Method: Convert(string, string)
Converts a raw YAML string into Markdown.

**Parameters**
- `yaml` — Raw YAML text to be processed. Must not be or whitespace.
- `dialectId` — Optional dialect identifier. When provided, the registry resolves the dialect by this ID; otherwise, it falls back to the first dialect whose `CanHandle` returns.

**Returns**

A Markdown [String](System.String.md) representing the parsed document. If the renderer returns, an empty string is returned.

**Exceptions**
- [Yaml2DocParseException](Models.Yaml2DocParseException.md) — Thrown when the YAML text is empty or cannot be parsed into a valid [PipelineDocument](Models.PipelineDocument.md). Low-level [YamlException](YamlDotNet.Core.YamlException.md) instances are wrapped in this exception.

