namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    /// <summary>
    /// A tracking service to inject in fakes to verify if certain code was run
    /// </summary>
    public interface IUnitTestTracker
    {
        /// <summary>
        /// Tracks the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        void Track(object obj);
    }
}
