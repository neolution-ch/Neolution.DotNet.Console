namespace Neolution.DotNet.Console.UnitTests
{
    using System;
    using System.Collections;
    using System.Text.Json;
    using Neolution.DotNet.Console.UnitTests.Stubs;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using NSubstitute;
    using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Some basic tests. Work in progress.
    /// </summary>
    public class ConsoleAppTests
    {
        [Fact]
        public void GivenServicesWithVariousServiceLifetimes_WhenRunningConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var console = DotNetConsole.CreateDefaultBuilder(InjectServicesWithVariousLifetimesOptions.Verb.Split(" "))
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ITransientServiceStub, TransientServiceStub>();
                    services.AddScoped<IScopedServiceStub, ScopedServiceStub>();
                    services.AddSingleton<ISingletonServiceStub, SingletonServiceStub>();
                })
                .UseCompositionRoot<CompositionRootStub>()
                .Build();

            // Act

            // Assert
            Should.NotThrow(() => console.Run());
        }

        /// <summary>
        /// Given the built console application, when calling with the verb only then should return expected result.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        [Theory]
        [AutoMockData]
        public void GivenBuiltConsoleApp_WhenCallingWithVerbOnly_ThenShouldReturnExpectedResult([Frozen] IUnitTestTracker tracker)
        {
            // Arrange
            const string args = "echo hello";
            var console = CreateConsoleAppWithTracking(args, tracker);

            // Act
            console.Run();

            // Assert
            var options = GetReceivedOptions<EchoOptions>(tracker);
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(false);
            options.Repeat.ShouldBe(1);
        }

        /// <summary>
        /// Given the built console application, when calling the verb with option then should return expected result.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        [Theory]
        [AutoMockData]
        public void GivenBuiltConsoleApp_WhenCallingVerbWithOption_ThenShouldReturnExpectedResult([Frozen] IUnitTestTracker tracker)
        {
            // Arrange
            const string args = "echo hello --upper";
            var console = CreateConsoleAppWithTracking(args, tracker);

            // Act
            console.Run();

            // Assert
            var options = GetReceivedOptions<EchoOptions>(tracker);
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(true);
            options.Repeat.ShouldBe(1);
        }

        /// <summary>
        /// Given the built console application, when calling with the verb and an option that takes value then should return expected result3.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        [Theory]
        [AutoMockData]
        public void GivenBuiltConsoleApp_WhenCallingWithOptionThatTakesValue_ThenShouldReturnExpectedResult3([Frozen] IUnitTestTracker tracker)
        {
            // Arrange
            const string args = "echo hello --upper --repeat 2";
            var console = CreateConsoleAppWithTracking(args, tracker);

            // Act
            console.Run();

            // Assert
            var options = GetReceivedOptions<EchoOptions>(tracker);
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(true);
            options.Repeat.ShouldBe(2);
        }

        /// <summary>
        /// Gets the received options from the specified tracker.
        /// </summary>
        /// <typeparam name="TOptions">The type of the options.</typeparam>
        /// <param name="tracker">The tracker.</param>
        /// <returns>The received options instance.</returns>
        private static TOptions GetReceivedOptions<TOptions>(IUnitTestTracker tracker)
        {
            return (TOptions)tracker.ReceivedCalls().SelectMany(x => x.GetArguments()).First();
        }

        /// <summary>
        /// Creates the console application with tracking.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="tracker">The tracker.</param>
        /// <returns>A built console app ready to run.</returns>
        private static IConsoleApp CreateConsoleAppWithTracking(string args, IUnitTestTracker tracker)
        {
            var builder = DotNetConsole.CreateDefaultBuilder(args.Split(" "))
                .ConfigureServices((context, services) =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IUnitTestTracker), tracker));
                })
                .UseCompositionRoot<CompositionRootFake>();

            return builder.Build();
        }
    }
}
