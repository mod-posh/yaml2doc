# YamlLoadException

Exception thrown when a YAML document cannot be loaded or parsed.

**Remarks**

Use this exception to signal errors encountered during reading or interpreting YAML, such as malformed content, unsupported constructs, or IO failures from the loader.

<a id="yaml2doc.core.parsing.yamlloadexception.#ctor(string,system.exception)"></a>

## Method: #ctor(string, Exception)

Initializes a new instance of the [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) class with a specified error message and a reference to the inner exception that is the cause of this exception.

**Parameters**

- `message` — A descriptive message that explains the reason for the failure.
- `innerException` — The exception that caused the current failure.

<a id="yaml2doc.core.parsing.yamlloadexception.#ctor(string)"></a>

## Method: #ctor(string)

Initializes a new instance of the [YamlLoadException](Yaml2Doc.Core.Parsing.YamlLoadException.md) class with a specified error message.

**Parameters**

- `message` — A descriptive message that explains the reason for the failure.
