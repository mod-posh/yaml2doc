# YamlLoader

Transforms a parsed YAML document into the neutral [PipelineDocument](Models.PipelineDocument.md) model.

**Remarks**

Expects the root of the YAML to be a mapping (object). Nested nodes are converted into dictionaries, lists, and strings to keep the model simple and loader-agnostic.

<a id="yaml2doc.core.parsing.yamlloader.convertmapping(yamldotnet.representationmodel.yamlmappingnode)"></a>
## Method: ConvertMapping(YamlMappingNode)
Converts a [YamlMappingNode](YamlDotNet.RepresentationModel.YamlMappingNode.md) into a dictionary of simple values.

**Parameters**
- `mapping` — The mapping node to convert.

**Returns**

A dictionary with string keys and converted values.

**Exceptions**
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown when a mapping key is not a scalar.

<a id="yaml2doc.core.parsing.yamlloader.convertnode(yamldotnet.representationmodel.yamlnode)"></a>
## Method: ConvertNode(YamlNode)
Converts a [YamlNode](YamlDotNet.RepresentationModel.YamlNode.md) into a simple CLR representation.

**Parameters**
- `node` — The YAML node to convert.

**Returns**

A [String](System.String.md) for scalars, [IDictionary<T1,T2>](System.Collections.Generic.IDictionary.md) for mappings, [IList<T1>](System.Collections.Generic.IList.md) for sequences, or when appropriate.

**Exceptions**
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown for unsupported node types.

<a id="yaml2doc.core.parsing.yamlloader.convertscalar(yamldotnet.representationmodel.yamlscalarnode)"></a>
## Method: ConvertScalar(YamlScalarNode)
Converts a [YamlScalarNode](YamlDotNet.RepresentationModel.YamlScalarNode.md) into a [String](System.String.md).

**Parameters**
- `scalar` — The scalar node to convert.

**Returns**

The scalar's string value, or.

<a id="yaml2doc.core.parsing.yamlloader.convertsequence(yamldotnet.representationmodel.yamlsequencenode)"></a>
## Method: ConvertSequence(YamlSequenceNode)
Converts a [YamlSequenceNode](YamlDotNet.RepresentationModel.YamlSequenceNode.md) into a list of simple values.

**Parameters**
- `sequence` — The sequence node to convert.

**Returns**

A list of converted values.

<a id="yaml2doc.core.parsing.yamlloader.load(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: Load(YamlDocumentContext)
Converts the parsed YAML represented by `context` into a neutral [PipelineDocument](Models.PipelineDocument.md) model.

**Parameters**
- `context` — The validated YAML document context to load from.

**Returns**

A populated [PipelineDocument](Models.PipelineDocument.md).

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown when the root node is not a mapping or contains invalid keys.

