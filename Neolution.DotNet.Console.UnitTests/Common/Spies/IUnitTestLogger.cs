using System.Collections.Generic;

namespace Neolution.DotNet.Console.UnitTests.Common.Spies
{
    /// <summary>
    /// A service to inject in fakes to log any object verify if certain code was run
    /// </summary>
    public interface IUnitTestLogger
    {
        public IList<object> LoggedObjects { get; }

        /// <summary>
        /// Tracks the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        void Log(object obj);
    }

    public class UnitTestLogger : IUnitTestLogger
    {
        public IList<object> LoggedObjects { get; } = new List<object>();

        public void Log(object obj)
        {
            LoggedObjects.Add(obj);
        }
    }
}
