﻿namespace Neolution.DotNet.Console.UnitTests.Spies
{
    using System.Collections.Generic;

    /// <summary>
    /// A service to inject in fakes to log any object verify if certain code was run
    /// </summary>
    public interface IUnitTestLogger
    {
        /// <summary>
        /// Gets the logged objects.
        /// </summary>
        IDictionary<string, object> LoggedObjects { get; }

        /// <summary>
        /// Logs the specified object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The object.</param>
        void Log(string key, object obj);
    }
}
