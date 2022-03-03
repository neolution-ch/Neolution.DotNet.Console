namespace Neolution.DotNet.Console
{
    using System;

    /// <inheritdoc />
    [Serializable]
    public class ConsoleAppException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConsoleAppException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ConsoleAppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppException"/> class.
        /// </summary>
        public ConsoleAppException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected ConsoleAppException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
