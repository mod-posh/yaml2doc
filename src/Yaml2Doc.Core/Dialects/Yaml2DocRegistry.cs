using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Dialects
{
    /// <summary>
    /// Registry that holds available YAML dialects and resolves the appropriate one for a given document.
    /// </summary>
    /// <remarks>
    /// Resolution can be forced to a specific dialect by identifier, or determined by querying each dialect's
    /// <see cref="IYamlDialect.CanHandle(YamlDocumentContext)"/> method in registration order.
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
        /// Thrown when a forced id is not registered, the forced dialect cannot handle the document,
        /// or no registered dialect can handle the document.
        /// </exception>
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

                if (!forced.CanHandle(context))
                    throw new InvalidOperationException(
                        $"Dialect '{forcedId}' is registered but cannot handle this document.");

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
        /// Thrown when no suitable dialect can be resolved or the forced dialect cannot handle the document.
        /// </exception>
        public PipelineDocument Parse(
            YamlDocumentContext context,
            string? forcedId = null)
        {
            var dialect = ResolveDialect(context, forcedId);
            return dialect.Parse(context);
        }

        /// <summary>
        /// Creates the default registry for v1 containing only the <see cref="StandardYamlDialect"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Yaml2DocRegistry"/> preconfigured with the standard dialect.
        /// </returns>
        public static Yaml2DocRegistry CreateDefault()
        {
            var loader = new YamlLoader();
            var standard = new StandardYamlDialect(loader);

            return new Yaml2DocRegistry(new[] { standard });
        }
    }
}
