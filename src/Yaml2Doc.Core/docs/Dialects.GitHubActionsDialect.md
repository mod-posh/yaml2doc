# GitHubActionsDialect

GitHub Actions-specific YAML dialect.

**Remarks**

Detection is heuristic-based and considers typical GitHub Actions workflow structure: - The document root must be a mapping. - Presence of root-level keys `on` and `jobs`. Parsing delegates to [YamlLoader](Parsing.YamlLoader.md) to produce a neutral [PipelineDocument](Models.PipelineDocument.md). The parsed document's [DialectId](Models.PipelineDocument.md#yaml2doc.core.models.pipelinedocument.dialectid) is set to `"gha"`. Implementations should be deterministic and must not mutate inputs.

<a id="yaml2doc.core.dialects.githubactionsdialect.#ctor(yaml2doc.core.parsing.yamlloader)"></a>
## Method: #ctor(YamlLoader)
Initializes a new instance of the [GitHubActionsDialect](Dialects.GitHubActionsDialect.md) class.

**Parameters**
- `loader` — The YAML loader used to transform documents into [PipelineDocument](Models.PipelineDocument.md) instances.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `loader` is.

<a id="yaml2doc.core.dialects.githubactionsdialect.canhandle(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: CanHandle(YamlDocumentContext)
Determines whether this dialect can interpret the given YAML document.

**Parameters**
- `context` — The loaded YAML document context to inspect. Must not be.

**Returns**

if the document appears to be a GitHub Actions workflow (root is a mapping and has `on` and `jobs` keys); otherwise,.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.

<a id="yaml2doc.core.dialects.githubactionsdialect.id"></a>
## Property: Id
Gets the stable identifier for this dialect.

<a id="yaml2doc.core.dialects.githubactionsdialect.parse(yaml2doc.core.parsing.yamldocumentcontext)"></a>
## Method: Parse(YamlDocumentContext)
Parses the YAML document into a [PipelineDocument](Models.PipelineDocument.md) using the configured loader.

**Parameters**
- `context` — The loaded YAML document context to parse. Must not be.

**Returns**

A populated [PipelineDocument](Models.PipelineDocument.md) representing the input YAML, with [DialectId](Models.PipelineDocument.md#yaml2doc.core.models.pipelinedocument.dialectid) set to `"gha"`.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `context` is.
- [YamlLoadException](Parsing.YamlLoadException.md) — Thrown when the document cannot be parsed into a valid model.

