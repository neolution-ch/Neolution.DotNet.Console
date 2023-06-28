namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    using System;

    /// <summary>
    /// Stub for a generic singleton service implementation.
    /// </summary>
    public class SingletonServiceStub : ISingletonServiceStub
    {
        /// <inheritdoc />
        public void DoSomething()
        {
            throw new NotSupportedException();
        }
    }
}
