namespace Neolution.DotNet.Console.UnitTests.Spies
{
    using System.Collections.Generic;

    /// <inheritdoc />
    public class UnitTestLogger : IUnitTestLogger
    {
        /// <inheritdoc />
        public IDictionary<string, object> LoggedObjects { get; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public void Log(string key, object obj)
        {
            this.LoggedObjects.Add(key, obj);
        }
    }
}
