using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Dialects
{
    /// <summary>
    /// Registry that holds available YAML dialects and resolves the appropriate one for a given document.
    /// </summary>
    /// <remarks>
    /// Dialect resolution proceeds in registration order unless a specific dialect identifier is forced.
    /// Instances are immutable after construction and safe for concurrent use.
    /// </remarks>
    public sealed class Yaml2DocRegistry
    {
        private readonly IReadOnlyList<IYamlDialect> _dialects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Yaml2DocRegistry"/> class with the provided dialects.
        /// </summary>
        /// <param name="dialects">
        /// The collection of dialects to register. Must not be <see langword="null"/> and must contain at least one element.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dialects"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when no dialects are provided.
        /// </exception>
        public Yaml2DocRegistry(IEnumerable<IYamlDialect> dialects)
        {
            if (dialects is null) throw new ArgumentNullException(nameof(dialects));

            var list = dialects.ToList();
            if (list.Count == 0)
                throw new ArgumentException("At least one dialect must be registered.", nameof(dialects));

            _dialects = list;
        }

        /// <summary>
        /// Gets the registered dialects in the registry.
        /// </summary>
        public IReadOnlyList<IYamlDialect> Dialects => _dialects;

        /// <summary>
        /// Resolves a dialect for the given document context, optionally forcing a specific dialect by id.
        /// </summary>
        /// <param name="context">The loaded YAML document context to evaluate. Must not be <see langword="null"/>.</param>
        /// <param name="forcedId">An optional dialect id to force selection (case-insensitive).</param>
        /// <returns>The matching <see cref="IYamlDialect"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a forced id is not registered, or no registered dialect can handle the document.
        /// </exception>
        /// <remarks>
        /// When <paramref name="forcedId"/> is provided and found, the corresponding dialect is returned
        /// without consulting <see cref="IYamlDialect.CanHandle(YamlDocumentContext)"/>. This allows callers
        /// to bypass heuristics when they know the target dialect.
        /// </remarks>
        public IYamlDialect ResolveDialect(
            YamlDocumentContext context,
            string? forcedId = null)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (!string.IsNullOrWhiteSpace(forcedId))
            {
                var forced = _dialects.FirstOrDefault(d =>
                    string.Equals(d.Id, forcedId, StringComparison.OrdinalIgnoreCase));

                if (forced is null)
                    throw new InvalidOperationException($"No dialect with id '{forcedId}' is registered.");

                // Trust the caller: if they force a dialect, use it even if CanHandle(...) would say "no".
                return forced;
            }

            var candidate = _dialects.FirstOrDefault(d => d.CanHandle(context));
            if (candidate is null)
                throw new InvalidOperationException("No registered dialect can handle this document.");

            return candidate;
        }

        /// <summary>
        /// Resolves an appropriate dialect and immediately parses the document into a <see cref="PipelineDocument"/>.
        /// </summary>
        /// <param name="context">The loaded YAML document context to parse. Must not be <see langword="null"/>.</param>
        /// <param name="forcedId">An optional dialect id to force selection (case-insensitive).</param>
        /// <returns>A populated <see cref="PipelineDocument"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no suitable dialect can be resolved or the forced id is not registered.
        /// </exception>
        public PipelineDocument Parse(
            YamlDocumentContext context,
            string? forcedId = null)
        {
            var dialect = ResolveDialect(context, forcedId);
            return dialect.Parse(context);
        }

        /// <summary>
        /// Creates the default registry for v1.1 containing the built-in YAML dialects.
        /// </summary>
        /// <remarks>
        /// Dialects are registered in order of specificity so that more specialized dialects
        /// (e.g., <see cref="AzurePipelinesDialect"/>, <see cref="GitHubActionsDialect"/>) are considered
        /// before the generic <see cref="StandardYamlDialect"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Yaml2DocRegistry"/> preconfigured with the built-in dialects.
        /// </returns>
        public static Yaml2DocRegistry CreateDefault()
        {
            var loader = new YamlLoader();

            var githubActions = new GitHubActionsDialect(loader);
            var azurePipelines = new AzurePipelinesDialect(loader);
            var standard = new StandardYamlDialect(loader);

            return new Yaml2DocRegistry(new IYamlDialect[]
            {
                // More specific first:
                azurePipelines,
                githubActions,
                // Generic catch-all:
                standard
            });
        }
    }
}
