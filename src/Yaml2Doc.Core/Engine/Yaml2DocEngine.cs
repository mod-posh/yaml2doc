using System;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Interfaces;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Core.Models;

namespace Yaml2Doc.Core.Engine
{
    /// <summary>
    /// Main orchestration entry point for converting YAML text into Markdown.
    /// </summary>
    /// <remarks>
    /// The engine:
    /// 1) Builds a <see cref="YamlDocumentContext"/> from the raw YAML.
    /// 2) Resolves a dialect via <see cref="Yaml2DocRegistry"/> (optionally by identifier).
    /// 3) Parses the context into a <see cref="PipelineDocument"/>.
    /// 4) Renders the document to Markdown using the configured <see cref="IMarkdownRenderer"/>.
    /// </remarks>
    public sealed class Yaml2DocEngine
    {
        private readonly Yaml2DocRegistry _registry;
        private readonly IMarkdownRenderer _markdownRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Yaml2DocEngine"/> class.
        /// </summary>
        /// <param name="registry">The registry that resolves dialects for parsing.</param>
        /// <param name="markdownRenderer">The renderer used to produce Markdown output.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="registry"/> or <paramref name="markdownRenderer"/> is <see langword="null"/>.
        /// </exception>
        public Yaml2DocEngine(Yaml2DocRegistry registry, IMarkdownRenderer markdownRenderer)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _markdownRenderer = markdownRenderer ?? throw new ArgumentNullException(nameof(markdownRenderer));
        }

        /// <summary>
        /// Converts a raw YAML string into Markdown.
        /// </summary>
        /// <param name="yaml">Raw YAML text to be processed. Must not be <see langword="null"/> or whitespace.</param>
        /// <param name="dialectId">
        /// Optional dialect identifier. When provided, the registry resolves
        /// the dialect by this ID; otherwise, it falls back to the first dialect
        /// whose <c>CanHandle</c> returns <see langword="true"/>.
        /// </param>
        /// <returns>
        /// A Markdown <see cref="string"/> representing the parsed document.
        /// If the renderer returns <see langword="null"/>, an empty string is returned.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="yaml"/> is <see langword="null"/> or whitespace.
        /// </exception>
        public string Convert(string yaml, string? dialectId = null)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                throw new ArgumentException("YAML text must not be null or whitespace.", nameof(yaml));
            }

            // 1. Build context from raw YAML
            var context = YamlDocumentContext.FromString(yaml);

            // 2. Resolve dialect (optionally by ID)
            var dialect = _registry.ResolveDialect(context, dialectId);

            // 3. Parse into PipelineDocument
            var document = dialect.Parse(context);

            // 4. Render to Markdown
            var markdown = _markdownRenderer.Render(document);

            return markdown ?? string.Empty;
        }
    }
}
