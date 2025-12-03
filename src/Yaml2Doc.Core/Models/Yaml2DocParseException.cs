using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaml2Doc.Core.Models
{
    /// <summary>
    /// Represents a failure to parse YAML input into a <see cref="PipelineDocument"/>.
    /// </summary>
    /// <remarks>
    /// Thrown when the YAML text is syntactically invalid, violates expected schema,
    /// or cannot be transformed into the neutral <see cref="PipelineDocument"/> model.
    /// </remarks>
    public sealed class Yaml2DocParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Yaml2DocParseException"/> class.
        /// </summary>
        /// <param name="message">A descriptive error message explaining the parse failure.</param>
        /// <param name="innerException">
        /// The underlying exception that caused the parse to fail, if available; otherwise <see langword="null"/>.
        /// </param>
        public Yaml2DocParseException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
