using System;
using System.Linq;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using YamlDotNet.RepresentationModel;

namespace Yaml2Doc.Core.Dialects
{
    /// <summary>
    /// Azure DevOps pipelines YAML dialect.
    /// </summary>
    /// <remarks>
    /// Detection is heuristic-based and considers typical ADO pipeline structure:
    /// - The document root must be a mapping.
    /// - Presence of root-level keys such as <c>trigger</c>, <c>pool</c>, <c>stages</c>, <c>jobs</c>, or <c>steps</c>.
    /// Parsing delegates to <see cref="YamlLoader"/> to produce a neutral <see cref="PipelineDocument"/>.
    /// Implementations should be deterministic and must not mutate inputs.
    /// </remarks>
    public sealed class AzurePipelinesDialect : IYamlDialect
    {
        private static readonly string[] KnownRootKeys =
        {
            "trigger",
            "pool",
            "stages",
            "jobs",
            "steps"
        };

        private readonly YamlLoader _loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzurePipelinesDialect"/> class.
        /// </summary>
        /// <param name="loader">The YAML loader used to transform documents into <see cref="PipelineDocument"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="loader"/> is <see langword="null"/>.</exception>
        public AzurePipelinesDialect(YamlLoader loader)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        /// <summary>
        /// Gets the stable identifier for this dialect.
        /// </summary>
        /// <remarks>Uses the short form <c>"ado"</c>.</remarks>
        public string Id => "ado";

        /// <summary>
        /// Determines whether this dialect can interpret the given YAML document.
        /// </summary>
        /// <param name="context">The loaded YAML document context to inspect. Must not be <see langword="null"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the root is a mapping and contains any known ADO keys
        /// (<c>trigger</c>, <c>pool</c>, <c>stages</c>, <c>jobs</c>, <c>steps</c>); otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        public bool CanHandle(YamlDocumentContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            // Require a mapping root for pipelines.
            if (context.RootNode is not YamlMappingNode mapping)
            {
                return false;
            }

            var rootKeys = mapping.Children
                .Keys
                .OfType<YamlScalarNode>() // only scalar keys are relevant
                .Select(k => k.Value)
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();

            if (rootKeys.Count == 0)
            {
                return false;
            }

            return rootKeys.Any(k =>
                KnownRootKeys.Contains(k!, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Parses the YAML document into a <see cref="PipelineDocument"/> using the configured loader.
        /// </summary>
        /// <param name="context">The loaded YAML document context to parse. Must not be <see langword="null"/>.</param>
        /// <returns>A populated <see cref="PipelineDocument"/> representing the input YAML.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="YamlLoadException">Thrown when the document cannot be parsed into a valid model.</exception>
        public PipelineDocument Parse(YamlDocumentContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            // Project into the neutral PipelineDocument, keeping ADO concepts in the Root dictionary.
            return _loader.Load(context);
        }
    }
}
