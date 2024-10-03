namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic transient service.
    /// </summary>
    public interface ITransientServiceStub
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DoSomethingAsync();
    }
}
