using System;

namespace Yaml2Doc.Core.Parsing
{
    /// <summary>
    /// Represents an error that occurs when a YAML document cannot be loaded or parsed.
    /// </summary>
    /// <remarks>
    /// Used to signal failures encountered during reading or interpreting YAML content,
    /// such as malformed input, unsupported constructs, or loader-related I/O issues.
    /// </remarks>
    public sealed class YamlLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlLoadException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A descriptive message explaining the reason for the failure.</param>
        public YamlLoadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlLoadException"/> class with a specified error message
        /// and a reference to the inner exception that caused this exception.
        /// </summary>
        /// <param name="message">A descriptive message explaining the reason for the failure.</param>
        /// <param name="innerException">The underlying exception that caused the current failure.</param>
        public YamlLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
