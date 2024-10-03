namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic scoped service implementation.
    /// </summary>
    public class ScopedServiceStub : IScopedServiceStub
    {
        /// <inheritdoc />
        public async Task DoSomethingAsync()
        {
            // do nothing
            await Task.CompletedTask;
        }
    }
}
