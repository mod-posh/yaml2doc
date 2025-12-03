# YamlDocumentContext

Thin wrapper around [YamlStream](YamlDotNet.RepresentationModel.YamlStream.md) that exposes the root node for a single YAML document.

**Remarks**

Provides helper factory methods to load a YAML document from a [String](System.String.md) or a [TextReader](System.IO.TextReader.md). Validates that the loaded document exists and that the root node is a mapping (object) to simplify downstream processing.

<a id="yaml2doc.core.parsing.yamldocumentcontext.#ctor(yamldotnet.representationmodel.yamlstream,yamldotnet.representationmodel.yamlnode)"></a>

## Method: #ctor(YamlStream, YamlNode)

Initializes a new instance of the [YamlDocumentContext](Yaml2Doc.Core.Parsing.YamlDocumentContext.md) class.

**Parameters**

- `stream` — The parsed YAML stream.
- `rootNode` — The root node of the first document.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `stream` or `rootNode` is.

<a id="yaml2doc.core.parsing.yamldocumentcontext.fromstring(string)"></a>

## Method: FromString(string)

Loads a YAML document from a string and returns a validated [YamlDocumentContext](Yaml2Doc.Core.Parsing.YamlDocumentContext.md).

**Parameters**

- `yaml` — The YAML content to parse.

**Returns**

A [YamlDocumentContext](Yaml2Doc.Core.Parsing.YamlDocumentContext.md) with a valid stream and root node.

**Exceptions**

- [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) — Thrown when the input is empty, contains no documents, has a null root node, or the root node is not a mapping.

<a id="yaml2doc.core.parsing.yamldocumentcontext.fromtextreader(system.io.textreader)"></a>

## Method: FromTextReader(TextReader)

Loads a YAML document from a [TextReader](System.IO.TextReader.md) and returns a validated [YamlDocumentContext](Yaml2Doc.Core.Parsing.YamlDocumentContext.md).

**Parameters**

- `reader` — The text reader providing YAML content.

**Returns**

A [YamlDocumentContext](Yaml2Doc.Core.Parsing.YamlDocumentContext.md) with a valid stream and root node.

**Exceptions**

- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `reader` is.
- [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) — Thrown when parsing fails, the stream contains no documents, the root node is null, or the root node is not a mapping.

<a id="yaml2doc.core.parsing.yamldocumentcontext.rootnode"></a>

## Property: RootNode

The root node of the first document in the stream.

<a id="yaml2doc.core.parsing.yamldocumentcontext.stream"></a>

## Property: Stream

The loaded [YamlStream](YamlDotNet.RepresentationModel.YamlStream.md) containing the parsed YAML content.
