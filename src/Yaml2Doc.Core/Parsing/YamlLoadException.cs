using System;

namespace Yaml2Doc.Core.Parsing
{
    /// <summary>
    /// Exception thrown when a YAML document cannot be loaded or parsed.
    /// </summary>
    /// <remarks>
    /// Use this exception to signal errors encountered during reading or interpreting YAML,
    /// such as malformed content, unsupported constructs, or IO failures from the loader.
    /// </remarks>
    public sealed class YamlLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlLoadException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A descriptive message that explains the reason for the failure.</param>
        public YamlLoadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlLoadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A descriptive message that explains the reason for the failure.</param>
        /// <param name="innerException">The exception that caused the current failure.</param>
        public YamlLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
