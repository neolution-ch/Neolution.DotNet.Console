namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic singleton service.
    /// </summary>
    public interface ISingletonServiceStub
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DoSomethingAsync();
    }
}
