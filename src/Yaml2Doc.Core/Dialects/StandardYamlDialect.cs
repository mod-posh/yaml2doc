using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Dialects 
{
    /// <summary>
    /// Default, catch-all YAML dialect that interprets generic pipeline YAML into the neutral <see cref="PipelineDocument"/> model.
    /// </summary>
    /// <remarks>
    /// This dialect currently accepts any YAML document with a mapping root and delegates parsing to <see cref="YamlLoader"/>.
    /// Future versions may introduce stricter detection or specialization. Instances are stateless and safe for concurrent use.
    /// </remarks>
    public sealed class StandardYamlDialect : IYamlDialect
    {
        private readonly YamlLoader _loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardYamlDialect"/> class.
        /// </summary>
        /// <param name="loader">
        /// The <see cref="YamlLoader"/> used to transform documents into <see cref="PipelineDocument"/> instances.
        /// Must not be <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="loader"/> is <see langword="null"/>.
        /// </exception>
        public StandardYamlDialect(YamlLoader loader)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        /// <summary>
        /// Gets the stable identifier for this dialect.
        /// </summary>
        public string Id => "standard";

        /// <summary>
        /// Indicates whether this dialect can handle the provided YAML document.
        /// </summary>
        /// <param name="context">The loaded YAML document context to inspect. Must not be <see langword="null"/>.</param>
        /// <returns>
        /// Always <see langword="true"/>, as this dialect is currently a catch-all.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        public bool CanHandle(YamlDocumentContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            return true;
        }

        /// <summary>
        /// Parses the YAML document into a <see cref="PipelineDocument"/> using the configured loader.
        /// </summary>
        /// <param name="context">The loaded YAML document context to parse. Must not be <see langword="null"/>.</param>
        /// <returns>
        /// A populated <see cref="PipelineDocument"/> representing the input YAML.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="YamlLoadException">
        /// Thrown when the document cannot be parsed into a valid model.
        /// </exception>
        public PipelineDocument Parse(YamlDocumentContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            return _loader.Load(context);
        }
    }
}
