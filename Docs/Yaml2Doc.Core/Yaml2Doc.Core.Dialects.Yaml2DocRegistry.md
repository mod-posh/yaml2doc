# Yaml2DocRegistry

Registry that holds available YAML dialects and resolves the appropriate one for a given document.

**Remarks**

Dialect resolution proceeds in registration order unless a specific dialect identifier is forced. Instances are immutable after construction and safe for concurrent use.

<a id="yaml2doc.core.dialects.yaml2docregistry.#ctor(system.collections.generic.ienumerable[yaml2doc.core.dialects.iyamldialect])"></a>

## Method: #ctor(IEnumerable<IYamlDialect>)

Initializes a new instance of the [Yaml2DocRegistry](Yaml2Doc.Core.Dialects.Yaml2DocRegistry.md) class with the provided dialects.

**Parameters**

- `dialects` — The collection of dialects to register. Must not be and must contain at least one element.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `dialects` is.
- [ArgumentException](System.ArgumentException.md) — Thrown when no dialects are provided.

<a id="yaml2doc.core.dialects.yaml2docregistry.createdefault"></a>

## Method: CreateDefault

Creates the default registry for v1.1 containing the built-in YAML dialects.

**Returns**

A [Yaml2DocRegistry](Yaml2Doc.Core.Dialects.Yaml2DocRegistry.md) preconfigured with the built-in dialects.

<a id="yaml2doc.core.dialects.yaml2docregistry.dialects"></a>

## Property: Dialects

Gets the registered dialects in the registry.

<a id="yaml2doc.core.dialects.yaml2docregistry.parse(yaml2doc.core.parsing.yamldocumentcontext,string)"></a>

## Method: Parse(YamlDocumentContext, string)

Resolves an appropriate dialect and immediately parses the document into a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Parameters**

- `context` — The loaded YAML document context to parse. Must not be.
- `forcedId` — An optional dialect id to force selection (case-insensitive).

**Returns**

A populated [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [InvalidOperationException](System.InvalidOperationException.md) — Thrown when no suitable dialect can be resolved or the forced id is not registered.

<a id="yaml2doc.core.dialects.yaml2docregistry.resolvedialect(yaml2doc.core.parsing.yamldocumentcontext,string)"></a>

## Method: ResolveDialect(YamlDocumentContext, string)

Resolves a dialect for the given document context, optionally forcing a specific dialect by id.

**Parameters**

- `context` — The loaded YAML document context to evaluate. Must not be.
- `forcedId` — An optional dialect id to force selection (case-insensitive).

**Returns**

The matching [IYamlDialect](Yaml2Doc.Core.Dialects.IYamlDialect.md).

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [InvalidOperationException](System.InvalidOperationException.md) — Thrown when a forced id is not registered, or no registered dialect can handle the document.
