using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaml2Doc.Core.Models
{
    /// <summary>
    /// Represents a neutral, in-memory view of a loaded YAML pipeline document.
    /// </summary>
    /// <remarks>
    /// The model is generic and not tied to any single CI/CD dialect.
    /// Top-level keys are stored in a case-insensitive map to simplify common CI configuration usage.
    /// Instances are mutable; avoid sharing and mutating the same instance across threads.
    /// </remarks>
    /// <example>
    /// <code>
    /// var doc = new PipelineDocument
    /// {
    ///     Name = "build"
    /// };
    ///
    /// doc["trigger"] = "main";
    /// doc["steps"] = new[]
    /// {
    ///     new { script = "dotnet restore" },
    ///     new { script = "dotnet build --configuration Release" }
    /// };
    /// </code>
    /// </example>
    public sealed class PipelineDocument
    {
        /// <summary>
        /// Optional logical name for the document (e.g., derived from a top-level YAML field or filename).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optional identifier of the dialect that produced this document (e.g., <c>standard</c>, <c>gha</c>, <c>ado</c>).
        /// </summary>
        public string? DialectId { get; set; }

        /// <summary>
        /// Root map of top-level YAML keys to values.
        /// </summary>
        /// <remarks>
        /// Values are stored as arbitrary objects so different loaders or dialect handlers
        /// can choose their own shapes (e.g., primitives, dictionaries, or custom types).
        /// Keys are matched using <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// </remarks>
        public IDictionary<string, object?> Root { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineDocument"/> class
        /// with a case-insensitive root key map.
        /// </summary>
        public PipelineDocument()
        {
            // Case-insensitive top-level keys are usually more convenient for CI configs.
            Root = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Provides convenient access to top-level keys in <see cref="Root"/>.
        /// </summary>
        /// <param name="key">The top-level YAML key to get or set.</param>
        /// <returns>The associated value if present; otherwise, <see langword="null"/>.</returns>
        /// <remarks>
        /// Getting a missing key returns <see langword="null"/> rather than throwing.
        /// Setting a key adds or replaces the value in the root map.
        /// Key comparison is case-insensitive.
        /// </remarks>
        public object? this[string key]
        {
            get => Root.TryGetValue(key, out var value) ? value : null;
            set => Root[key] = value;
        }
    }
}
