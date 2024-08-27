namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Stub for a generic singleton service implementation.
    /// </summary>
    public class SingletonServiceStub : ISingletonServiceStub
    {
        /// <inheritdoc />
        public async Task DoSomethingAsync()
        {
            // do nothing
            await Task.CompletedTask;
        }
    }
}
