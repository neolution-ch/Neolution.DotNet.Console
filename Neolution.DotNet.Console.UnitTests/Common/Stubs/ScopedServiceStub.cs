namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    using System;

    /// <summary>
    /// Stub for a generic scoped service implementation.
    /// </summary>
    public class ScopedServiceStub : IScopedServiceStub
    {
        /// <inheritdoc />
        public void DoSomething()
        {
            throw new NotSupportedException();
        }
    }
}
