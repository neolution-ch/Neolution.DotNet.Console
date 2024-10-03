namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic scoped service.
    /// </summary>
    public interface IScopedServiceStub
    {
        /// <summary>
        /// Does something.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DoSomethingAsync();
    }
}
