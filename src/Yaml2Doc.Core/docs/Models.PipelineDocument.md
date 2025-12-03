# PipelineDocument

Represents a neutral, in-memory view of a loaded YAML pipeline document.

**Remarks**

The model is generic and not tied to any single CI/CD dialect. Top-level keys are stored in a case-insensitive map to simplify common CI configuration usage. Instances are mutable, but consumers should avoid mutating shared instances across threads.

**Example**

```csharp
             var doc = new PipelineDocument
             {
                 Name = "build"
             };
            
             doc["trigger"] = "main";
             doc["steps"] = new[]
             {
                 new { script = "dotnet restore" },
                 new { script = "dotnet build --configuration Release" }
             };
             
```

<a id="yaml2doc.core.models.pipelinedocument.#ctor"></a>
## Method: #ctor
Initializes a new instance of the [PipelineDocument](Models.PipelineDocument.md) class with a case-insensitive root key map.

<a id="yaml2doc.core.models.pipelinedocument.dialectid"></a>
## Property: DialectId
Optional identifier of the dialect that produced this document (e.g., `standard`, `gha`, `ado`).

<a id="yaml2doc.core.models.pipelinedocument.item(string)"></a>
## Property: Item(string)
Provides convenient access to top-level keys in [Root](Models.PipelineDocument.md#yaml2doc.core.models.pipelinedocument.root).

**Parameters**
- `key` — The top-level YAML key to get or set.

**Returns**

The associated value if present; otherwise,.

<a id="yaml2doc.core.models.pipelinedocument.name"></a>
## Property: Name
Optional logical name for the document (e.g., derived from a top-level YAML field or filename).

<a id="yaml2doc.core.models.pipelinedocument.root"></a>
## Property: Root
Root map of top-level YAML keys to values.

