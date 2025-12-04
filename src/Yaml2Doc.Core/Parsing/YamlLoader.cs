using System;
using System.Collections.Generic;
using Yaml2Doc.Core.Models;
using YamlDotNet.RepresentationModel;

namespace Yaml2Doc.Core.Parsing
{
    /// <summary>
    /// Transforms a parsed YAML document into the neutral <see cref="PipelineDocument"/> model.
    /// </summary>
    /// <remarks>
    /// Expects the document root to be a mapping (object). Nested nodes are converted into
    /// dictionaries, lists, and strings to keep the model simple and loader-agnostic.
    /// The loader does not mutate inputs and produces deterministic results for a given YAML.
    /// </remarks>
    public sealed class YamlLoader
    {
        /// <summary>
        /// Converts the parsed YAML represented by <paramref name="context"/> into a neutral
        /// <see cref="PipelineDocument"/> model.
        /// </summary>
        /// <param name="context">The validated YAML document context to load from. Must not be <see langword="null"/>.</param>
        /// <returns>A populated <see cref="PipelineDocument"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="YamlLoadException">Thrown when the root node is not a mapping or a root key is invalid.</exception>
        public PipelineDocument Load(YamlDocumentContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.RootNode is not YamlMappingNode mapping)
            {
                throw new YamlLoadException("Root YAML node must be a mapping (object).");
            }

            var document = new PipelineDocument();

            // Populate the root dictionary from the mapping.
            foreach (var entry in mapping.Children)
            {
                if (entry.Key is not YamlScalarNode keyScalar)
                {
                    throw new YamlLoadException("Non-scalar mapping key encountered at root.");
                }

                var key = keyScalar.Value ?? string.Empty;
                var value = ConvertNode(entry.Value);

                document.Root[key] = value;
            }

            // Set Name from root["name"] (case-insensitive due to the dictionary comparer).
            if (document["name"] is string nameStr &&
                !string.IsNullOrWhiteSpace(nameStr))
            {
                document.Name = nameStr;
            }

            return document;
        }

        /// <summary>
        /// Converts a <see cref="YamlNode"/> into a simple CLR representation.
        /// </summary>
        /// <param name="node">The YAML node to convert.</param>
        /// <returns>
        /// A <see cref="string"/> for scalars, an <see cref="IDictionary{TKey, TValue}"/> for mappings,
        /// an <see cref="IList{T}"/> for sequences, or <see langword="null"/> when appropriate.
        /// </returns>
        /// <exception cref="YamlLoadException">Thrown for unsupported node types.</exception>
        private static object? ConvertNode(YamlNode node)
        {
            return node switch
            {
                YamlScalarNode scalar => ConvertScalar(scalar),
                YamlMappingNode mapping => ConvertMapping(mapping),
                YamlSequenceNode seq => ConvertSequence(seq),
                _ => throw new YamlLoadException(
                    $"Unsupported YAML node type: {node.GetType().Name}")
            };
        }

        /// <summary>
        /// Converts a <see cref="YamlMappingNode"/> into a dictionary of simple values.
        /// </summary>
        /// <param name="mapping">The mapping node to convert.</param>
        /// <returns>A dictionary with string keys and converted values.</returns>
        /// <exception cref="YamlLoadException">Thrown when a mapping key is not a scalar.</exception>
        private static IDictionary<string, object?> ConvertMapping(YamlMappingNode mapping)
        {
            // Nested mappings use ordinal comparison for keys.
            var dict = new Dictionary<string, object?>(StringComparer.Ordinal);

            foreach (var entry in mapping.Children)
            {
                if (entry.Key is not YamlScalarNode keyScalar)
                {
                    throw new YamlLoadException("Non-scalar mapping key encountered.");
                }

                var key = keyScalar.Value ?? string.Empty;
                var value = ConvertNode(entry.Value);

                dict[key] = value;
            }

            return dict;
        }

        /// <summary>
        /// Converts a <see cref="YamlSequenceNode"/> into a list of simple values.
        /// </summary>
        /// <param name="sequence">The sequence node to convert.</param>
        /// <returns>A list of converted values.</returns>
        private static IList<object?> ConvertSequence(YamlSequenceNode sequence)
        {
            var list = new List<object?>();

            foreach (var node in sequence.Children)
            {
                list.Add(ConvertNode(node));
            }

            return list;
        }

        /// <summary>
        /// Converts a <see cref="YamlScalarNode"/> into its string value.
        /// </summary>
        /// <param name="scalar">The scalar node to convert.</param>
        /// <returns>The scalar's string value, or <see langword="null"/>.</returns>
        /// <remarks>
        /// Type coercion (for example, booleans or numbers) is intentionally deferred; values remain stringly-typed.
        /// </remarks>
        private static object? ConvertScalar(YamlScalarNode scalar)
        {
            // Keep it simple and stringly-typed for now.
            // Type coercion (bool/int/etc.) can be added later if needed.
            return scalar.Value;
        }
    }
}
