using System;
using System.Linq;
using System.Text;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Markdown.Interfaces;

namespace Yaml2Doc.Markdown
{
    /// <summary>
    /// Baseline Markdown renderer that produces a simple overview of a <see cref="PipelineDocument"/>.
    /// </summary>
    /// <remarks>
    /// Renders:
    /// - H1 as the document name if present; otherwise, the title is &quot;YAML Document&quot;.
    /// - A &quot;Root Keys&quot; section listing top-level keys.
    /// The renderer does not mutate the input <see cref="PipelineDocument"/> and produces deterministic output
    /// for a given input.
    /// </remarks>
    public sealed class BasicMarkdownRenderer : IMarkdownRenderer
    {
        /// <summary>
        /// Converts the provided <paramref name="document"/> into a basic Markdown representation.
        /// </summary>
        /// <param name="document">
        /// The <see cref="PipelineDocument"/> to render. Must not be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A Markdown string containing the document title and a bullet list of root keys.
        /// If there are no root keys, the list is replaced with <c>_(no root keys)_</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> is <see langword="null"/>.
        /// </exception>
        public string Render(PipelineDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var sb = new StringBuilder();

            // Title
            var title = string.IsNullOrWhiteSpace(document.Name)
                ? "YAML Document"
                : document.Name;

            sb.Append("# ")
              .AppendLine(title)
              .AppendLine();

            // Root keys section
            sb.AppendLine("## Root Keys")
              .AppendLine();

            if (document.Root is null || document.Root.Count == 0)
            {
                sb.AppendLine("_(no root keys)_");
                return sb.ToString();
            }

            // Use the dictionary's keys; order is whatever the caller inserted
            foreach (var key in document.Root.Keys)
            {
                sb.Append("- ")
                  .AppendLine(key);
            }

            return sb.ToString();
        }
    }
}
