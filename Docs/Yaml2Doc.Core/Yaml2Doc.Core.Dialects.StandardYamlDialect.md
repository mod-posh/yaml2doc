# StandardYamlDialect

Default, catch-all YAML dialect that interprets generic pipeline YAML into the neutral [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) model.

**Remarks**

This dialect currently accepts any YAML document with a mapping root and delegates parsing to [YamlLoader](Yaml2Doc.Core.Parsing.YamlLoader.md). Future versions may introduce stricter detection or specialization. Instances are stateless and safe for concurrent use.

<a id="yaml2doc.core.dialects.standardyamldialect.#ctor(yaml2doc.core.parsing.yamlloader)"></a>

## Method: #ctor(YamlLoader)

Initializes a new instance of the [StandardYamlDialect](Yaml2Doc.Core.Dialects.StandardYamlDialect.md) class.

**Parameters**

- `loader` — The [YamlLoader](Yaml2Doc.Core.Parsing.YamlLoader.md) used to transform documents into [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) instances. Must not be.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `loader` is.

<a id="yaml2doc.core.dialects.standardyamldialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)"></a>

## Method: CanHandle(YamlDocumentContext)

Indicates whether this dialect can handle the provided YAML document.

**Parameters**

- `context` — The loaded YAML document context to inspect. Must not be.

**Returns**

Always, as this dialect is currently a catch-all.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.

<a id="yaml2doc.core.dialects.standardyamldialect.id"></a>

## Property: Id

Gets the stable identifier for this dialect.

<a id="yaml2doc.core.dialects.standardyamldialect.parse(yaml2doc.core.parsing.yamldocumentcontext)"></a>

## Method: Parse(YamlDocumentContext)

Parses the YAML document into a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) using the configured loader.

**Parameters**

- `context` — The loaded YAML document context to parse. Must not be.

**Returns**

A populated [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) representing the input YAML.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) — Thrown when the document cannot be parsed into a valid model.
