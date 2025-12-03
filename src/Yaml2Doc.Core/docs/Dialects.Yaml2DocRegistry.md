# Yaml2DocRegistry

Registry that holds available YAML dialects and resolves the appropriate one for a given document.

**Remarks**

Resolution can be forced to a specific dialect by id, or determined by querying each dialect's [CanHandle(YamlDocumentContext)](Dialects.IYamlDialect.md#yaml2doc.core.dialects.iyamldialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)) method in order.

<a id="yaml2doc.core.dialects.yaml2docregistry.#ctor(system.collections.generic.ienumerable[yaml2doc.core.dialects.iyamldialect])"></a>
## Method: #ctor(IEnumerable<IYamlDialect>)
Initializes a new instance of the [Yaml2DocRegistry](Dialects.Yaml2DocRegistry.md) class with the provided dialects.

**Parameters**
- `dialects` — The collection of dialects to register.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `dialects` is null.
- [ArgumentException](System.ArgumentException.md) — Thrown when no dialects are provided.

<a id="yaml2doc.core.dialects.yaml2docregistry.createdefault"></a>
## Method: CreateDefault
Creates the default registry for v1 containing only the [StandardYamlDialect](Dialects.StandardYamlDialect.md).

**Returns**

A [Yaml2DocRegistry](Dialects.Yaml2DocRegistry.md) preconfigured with the standard dialect.

<a id="yaml2doc.core.dialects.yaml2docregistry.dialects"></a>
## Property: Dialects
Gets the registered dialects in the registry.

<a id="yaml2doc.core.dialects.yaml2docregistry.parse(yaml2doc.core.parsing.yamldocumentcontext,string)"></a>
## Method: Parse(YamlDocumentContext, string)
Resolves an appropriate dialect and immediately parses the document into a [PipelineDocument](Models.PipelineDocument.md).

**Parameters**
- `context` — The loaded YAML document context to parse.
- `forcedId` — An optional dialect id to force selection (case-insensitive).

**Returns**

A populated [PipelineDocument](Models.PipelineDocument.md).

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is null.
- [InvalidOperationException](System.InvalidOperationException.md) — Thrown when no suitable dialect can be resolved or the forced dialect cannot handle the document.

<a id="yaml2doc.core.dialects.yaml2docregistry.resolvedialect(yaml2doc.core.parsing.yamldocumentcontext,string)"></a>
## Method: ResolveDialect(YamlDocumentContext, string)
Resolves a dialect for the given document context, optionally forcing a specific dialect by id.

**Parameters**
- `context` — The loaded YAML document context to evaluate.
- `forcedId` — An optional dialect id to force selection (case-insensitive).

**Returns**

The matching [IYamlDialect](Dialects.IYamlDialect.md).

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is null.
- [InvalidOperationException](System.InvalidOperationException.md) — Thrown when a forced id is not registered, the forced dialect cannot handle the document, or no registered dialect can handle the document.

