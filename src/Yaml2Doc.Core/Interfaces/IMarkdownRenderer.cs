using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaml2Doc.Core.Models;

namespace Yaml2Doc.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for rendering a <see cref="PipelineDocument"/> into Markdown.
    /// </summary>
    /// <remarks>
    /// Implementations should be deterministic for the same input, avoid mutating the provided
    /// <see cref="PipelineDocument"/>, and may be called multiple times with different documents.
    /// </remarks>
    public interface IMarkdownRenderer
    {
        /// <summary>
        /// Renders the specified <paramref name="document"/> into a Markdown string.
        /// </summary>
        /// <param name="document">
        /// The <see cref="PipelineDocument"/> to render. Must not be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the Markdown representation of the provided
        /// <see cref="PipelineDocument"/>. Never returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> is <see langword="null"/>.
        /// </exception>
        string Render(PipelineDocument document);
    }
}
