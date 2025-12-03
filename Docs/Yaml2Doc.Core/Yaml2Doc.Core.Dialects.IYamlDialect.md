# IYamlDialect

Defines a strategy for interpreting a YAML pipeline document for a specific CI/CD dialect.

**Remarks**

Implementations should identify whether they can handle a given document and, if so, parse it into the neutral [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) model. Implementations are expected to be stateless, avoid mutating the provided inputs, and produce deterministic results for the same input.

<a id="yaml2doc.core.dialects.iyamldialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)"></a>

## Method: CanHandle(YamlDocumentContext)

Determines whether this dialect can interpret the given YAML document.

**Parameters**

- `context` — The loaded YAML document context to inspect. Must not be.

**Returns**

if this dialect can handle the document; otherwise,.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.

<a id="yaml2doc.core.dialects.iyamldialect.id"></a>

## Property: Id

Stable identifier for the dialect (e.g., "standard", "github", "ado").

<a id="yaml2doc.core.dialects.iyamldialect.parse(yaml2doc.core.parsing.yamldocumentcontext)"></a>

## Method: Parse(YamlDocumentContext)

Parses the YAML document into a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Parameters**

- `context` — The loaded YAML document context to parse. Must not be.

**Returns**

A populated [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) representing the input YAML.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) — Thrown when the document is not valid for this dialect or contains unsupported constructs.
