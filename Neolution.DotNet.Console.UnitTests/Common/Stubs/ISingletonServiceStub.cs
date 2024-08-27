namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
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
