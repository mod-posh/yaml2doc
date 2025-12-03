using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Dialects;

/// <summary>
/// Defines a strategy for interpreting a YAML pipeline document for a specific CI/CD dialect.
/// </summary>
/// <remarks>
/// Implementations should identify whether they can handle a given document and, if so,
/// parse it into the neutral <see cref="PipelineDocument"/> model.
/// </remarks>
public interface IYamlDialect
{
    /// <summary>
    /// Stable identifier for the dialect (e.g., "standard", "github", "ado").
    /// </summary>
    /// <remarks>
    /// This value should be short, lowercase, and unique across dialect implementations.
    /// </remarks>
    string Id { get; }

    /// <summary>
    /// Determines whether this dialect can interpret the given YAML document.
    /// </summary>
    /// <param name="context">The loaded YAML document context to inspect.</param>
    /// <returns><see langword="true"/> if this dialect can handle the document; otherwise, <see langword="false"/>.</returns>
    bool CanHandle(YamlDocumentContext context);

    /// <summary>
    /// Parses the YAML document into a <see cref="PipelineDocument"/>.
    /// </summary>
    /// <param name="context">The loaded YAML document context to parse.</param>
    /// <returns>A populated <see cref="PipelineDocument"/>.</returns>
    /// <exception cref="YamlLoadException">
    /// Thrown when the document is not valid for this dialect or contains unsupported constructs.
    /// </exception>
    PipelineDocument Parse(YamlDocumentContext context);
}
