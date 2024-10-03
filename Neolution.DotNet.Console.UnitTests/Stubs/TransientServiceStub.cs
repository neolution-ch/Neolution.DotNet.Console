namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic transient service implementation.
    /// </summary>
    public class TransientServiceStub : ITransientServiceStub
    {
        /// <inheritdoc />
        public async Task DoSomethingAsync()
        {
            // do nothing
            await Task.CompletedTask;
        }
    }
}
