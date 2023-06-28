namespace Neolution.DotNet.Console.UnitTests.Common.Spies
{
    using System.Collections.Generic;

    /// <inheritdoc />
    public class UnitTestLogger : IUnitTestLogger
    {
        /// <inheritdoc />
        public IList<object> LoggedObjects { get; } = new List<object>();

        /// <inheritdoc />
        public void Log(object obj)
        {
            this.LoggedObjects.Add(obj);
        }
    }
}
