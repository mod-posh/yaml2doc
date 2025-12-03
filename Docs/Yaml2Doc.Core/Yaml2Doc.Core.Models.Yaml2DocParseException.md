# Yaml2DocParseException

Represents a failure to parse YAML input into a [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md).

**Remarks**

Thrown when the YAML text is syntactically invalid, violates expected schema, or cannot be transformed into the neutral [PipelineDocument](Yaml2Doc.Core.Models.PipelineDocument.md) model.

<a id="yaml2doc.core.models.yaml2docparseexception.#ctor(string,system.exception)"></a>

## Method: #ctor(string, Exception)

Initializes a new instance of the [Yaml2DocParseException](Yaml2Doc.Core.Models.Yaml2DocParseException.md) class.

**Parameters**

- `message` — A descriptive error message explaining the parse failure.
- `innerException` — The underlying exception that caused the parse to fail, if available; otherwise.
