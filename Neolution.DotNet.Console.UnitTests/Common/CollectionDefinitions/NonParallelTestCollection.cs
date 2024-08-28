﻿namespace Neolution.DotNet.Console.UnitTests.Common.CollectionDefinitions
{
    using Xunit;

    /// <summary>
    /// Collection definition for tests that should not run in parallel.
    /// </summary>
    [CollectionDefinition("Non-Parallel Tests", DisableParallelization = true)]
    public class NonParallelTestCollection
    {
    }
}
