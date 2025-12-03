# IYamlDialect

Defines a strategy for interpreting a YAML pipeline document for a specific CI/CD dialect.

**Remarks**

Implementations should identify whether they can handle a given document and, if so, parse it into the neutral [PipelineDocument](Models.PipelineDocument.md) model.

<a id="yaml2doc.core.dialects.iyamldialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: CanHandle(YamlDocumentContext)
Determines whether this dialect can interpret the given YAML document.

**Parameters**
- `context` — The loaded YAML document context to inspect.

**Returns**

if this dialect can handle the document; otherwise,.

<a id="yaml2doc.core.dialects.iyamldialect.id"></a>
## Property: Id
Stable identifier for the dialect (e.g., "standard", "github", "ado").

<a id="yaml2doc.core.dialects.iyamldialect.parse(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: Parse(YamlDocumentContext)
Parses the YAML document into a [PipelineDocument](Models.PipelineDocument.md).

**Parameters**
- `context` — The loaded YAML document context to parse.

**Returns**

A populated [PipelineDocument](Models.PipelineDocument.md).

**Exceptions**
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown when the document is not valid for this dialect or contains unsupported constructs.

