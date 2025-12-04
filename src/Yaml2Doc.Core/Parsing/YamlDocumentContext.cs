using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Yaml2Doc.Core.Parsing
{
    /// <summary>
    /// Thin wrapper around <see cref="YamlStream"/> that exposes the root node for a single YAML document.
    /// </summary>
    /// <remarks>
    /// Provides factory methods to load a YAML document from a <see cref="string"/> or a <see cref="TextReader"/>.
    /// Validates that the stream contains at least one document and that the root node is a mapping (object),
    /// simplifying downstream processing.
    /// </remarks>
    public sealed class YamlDocumentContext
    {
        /// <summary>
        /// Gets the loaded <see cref="YamlStream"/> containing the parsed YAML content.
        /// </summary>
        public YamlStream Stream { get; }

        /// <summary>
        /// Gets the root node of the first document in the stream.
        /// </summary>
        /// <remarks>
        /// Guaranteed to be a <see cref="YamlMappingNode"/> when created via the provided factory methods.
        /// </remarks>
        public YamlNode RootNode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlDocumentContext"/> class.
        /// </summary>
        /// <param name="stream">The parsed YAML stream.</param>
        /// <param name="rootNode">The root node of the first document.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stream"/> or <paramref name="rootNode"/> is <see langword="null"/>.
        /// </exception>
        private YamlDocumentContext(YamlStream stream, YamlNode rootNode)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }

        /// <summary>
        /// Loads a YAML document from a string and returns a validated <see cref="YamlDocumentContext"/>.
        /// </summary>
        /// <param name="yaml">The YAML content to parse. Must not be <see langword="null"/> or whitespace.</param>
        /// <returns>A <see cref="YamlDocumentContext"/> with a valid stream and mapping root node.</returns>
        /// <exception cref="YamlLoadException">
        /// Thrown when the input is empty, contains no documents, has a null root node,
        /// or the root node is not a mapping.
        /// </exception>
        public static YamlDocumentContext FromString(string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                throw new YamlLoadException("YAML input is empty.");
            }

            using var reader = new StringReader(yaml);
            return FromTextReader(reader);
        }

        /// <summary>
        /// Loads a YAML document from a <see cref="TextReader"/> and returns a validated <see cref="YamlDocumentContext"/>.
        /// </summary>
        /// <param name="reader">The text reader providing YAML content. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="YamlDocumentContext"/> with a valid stream and mapping root node.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="reader"/> is <see langword="null"/>.</exception>
        /// <exception cref="YamlLoadException">
        /// Thrown when parsing fails, the stream contains no documents, the root node is <see langword="null"/>,
        /// or the root node is not a mapping.
        /// </exception>
        public static YamlDocumentContext FromTextReader(TextReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var stream = new YamlStream();
            try
            {
                stream.Load(reader);
            }
            catch (YamlException ex)
            {
                throw new YamlLoadException("Failed to parse YAML.", ex);
            }

            if (stream.Documents.Count == 0)
            {
                throw new YamlLoadException("YAML contains no documents.");
            }

            var root = stream.Documents[0].RootNode;
            if (root is null)
            {
                throw new YamlLoadException("YAML root node is null.");
            }

            if (root is not YamlMappingNode)
            {
                throw new YamlLoadException("Root YAML node must be a mapping (object).");
            }

            return new YamlDocumentContext(stream, root);
        }
    }
}
