namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    using System;

    /// <summary>
    /// Stub for a generic transient service implementation.
    /// </summary>
    public class TransientServiceStub : ITransientServiceStub
    {
        /// <inheritdoc />
        public void DoSomething()
        {
            throw new NotSupportedException();
        }
    }
}
