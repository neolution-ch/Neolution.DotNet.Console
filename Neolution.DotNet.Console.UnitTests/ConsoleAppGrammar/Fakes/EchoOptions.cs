﻿namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using CommandLine;

    /// <summary>
    /// Fake options for <see cref="EchoCommand"/>
    /// </summary>
    [Verb("echo")]
    public class EchoOptions
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [Value(0)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the echo should be printed in upper case.
        /// </summary>
        /// <value>
        ///   <c>true</c> if upper; otherwise, <c>false</c>.
        /// </value>
        [Option]
        public bool Upper { get; set; }

        /// <summary>
        /// Gets or sets the repeat.
        /// </summary>
        [Option(Default = 1)]
        public int Repeat { get; set; }
    }
}
