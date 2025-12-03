# StandardYamlDialect

Default, catch-all YAML dialect that interprets generic pipeline YAML into the neutral [PipelineDocument](Models.PipelineDocument.md) model.

**Remarks**

This dialect currently accepts any YAML document with a mapping root and delegates parsing to [YamlLoader](Parsing.YamlLoader.md). Future versions may introduce stricter detection or specialization.

<a id="yaml2doc.core.dialects.standardyamldialect.#ctor(yaml2doc.core.parsing.yamlloader)"></a>
## Method: #ctor(YamlLoader)
Initializes a new instance of the [StandardYamlDialect](Dialects.StandardYamlDialect.md) class.

**Parameters**
- `loader` — The YAML loader used to transform documents into [PipelineDocument](Models.PipelineDocument.md) instances.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `loader` is null.

<a id="yaml2doc.core.dialects.standardyamldialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: CanHandle(YamlDocumentContext)
Indicates whether this dialect can handle the provided YAML document.

**Parameters**
- `context` — The loaded YAML document context to inspect.

**Returns**

Always, as this dialect is currently a catch-all.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is null.

<a id="yaml2doc.core.dialects.standardyamldialect.id"></a>
## Property: Id
Stable identifier for this dialect.

<a id="yaml2doc.core.dialects.standardyamldialect.parse(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: Parse(YamlDocumentContext)
Parses the YAML document into a [PipelineDocument](Models.PipelineDocument.md) using the configured loader.

**Parameters**
- `context` — The loaded YAML document context to parse.

**Returns**

A populated [PipelineDocument](Models.PipelineDocument.md).

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is null.
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown when the document cannot be parsed into a valid model.

