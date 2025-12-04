using System;
using System.Linq;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using YamlDotNet.RepresentationModel;

namespace Yaml2Doc.Core.Dialects
{
    /// <summary>
    /// GitHub Actions-specific YAML dialect.
    /// </summary>
    /// <remarks>
    /// Detection is heuristic-based and considers typical GitHub Actions workflow structure:
    /// - The document root must be a mapping.
    /// - Presence of root-level keys <c>on</c> and <c>jobs</c>.
    /// Parsing delegates to <see cref="YamlLoader"/> to produce a neutral <see cref="PipelineDocument"/>.
    /// The parsed document's <see cref="PipelineDocument.DialectId"/> is set to <c>"gha"</c>.
    /// Implementations should be deterministic and must not mutate inputs.
    /// </remarks>
    public sealed class GitHubActionsDialect : IYamlDialect
    {
        private readonly YamlLoader _loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubActionsDialect"/> class.
        /// </summary>
        /// <param name="loader">The YAML loader used to transform documents into <see cref="PipelineDocument"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="loader"/> is <see langword="null"/>.</exception>
        public GitHubActionsDialect(YamlLoader loader)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        /// <summary>
        /// Gets the stable identifier for this dialect.
        /// </summary>
        /// <remarks>Uses the short form <c>"gha"</c>.</remarks>
        public string Id => "gha";

        /// <summary>
        /// Determines whether this dialect can interpret the given YAML document.
        /// </summary>
        /// <param name="context">The loaded YAML document context to inspect. Must not be <see langword="null"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the document appears to be a GitHub Actions workflow (root is a mapping and has <c>on</c> and <c>jobs</c> keys);
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        public bool CanHandle(YamlDocumentContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Require a mapping root for workflows.
            if (context.RootNode is not YamlMappingNode)
            {
                return false;
            }

            // Use the generic loader and inspect the resulting PipelineDocument, avoiding tight coupling to raw nodes.
            PipelineDocument document;
            document = _loader.Load(context);

            if (document.Root is null || document.Root.Count == 0)
            {
                return false;
            }

            var keys = document.Root.Keys.ToArray();

            var hasOn = keys.Any(k => string.Equals(k, "on", StringComparison.OrdinalIgnoreCase));
            var hasJobs = keys.Any(k => string.Equals(k, "jobs", StringComparison.OrdinalIgnoreCase));

            return hasOn && hasJobs;
        }

        /// <summary>
        /// Parses the YAML document into a <see cref="PipelineDocument"/> using the configured loader.
        /// </summary>
        /// <param name="context">The loaded YAML document context to parse. Must not be <see langword="null"/>.</param>
        /// <returns>
        /// A populated <see cref="PipelineDocument"/> representing the input YAML, with
        /// <see cref="PipelineDocument.DialectId"/> set to <c>"gha"</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="YamlLoadException">Thrown when the document cannot be parsed into a valid model.</exception>
        public PipelineDocument Parse(YamlDocumentContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Generic load into PipelineDocument.
            var document = _loader.Load(context);
            document.DialectId = Id;
            return document;
        }
    }
}
