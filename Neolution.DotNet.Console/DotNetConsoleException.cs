// Disable warnings that are conflicting with a breaking change from .NET 8 SDK (SYSLIB0051)
#pragma warning disable S4027
#pragma warning disable S3925
namespace Neolution.DotNet.Console
{
    using System;

    /// <inheritdoc />
    [Serializable]
    public class DotNetConsoleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsoleException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DotNetConsoleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsoleException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public DotNetConsoleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsoleException"/> class.
        /// </summary>
        public DotNetConsoleException()
        {
        }
    }
}
