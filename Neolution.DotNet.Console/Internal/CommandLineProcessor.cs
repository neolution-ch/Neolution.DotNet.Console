namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CommandLine;

    /// <summary>
    /// Handles command line argument parsing and validation.
    /// </summary>
    internal static class CommandLineProcessor
    {
        /// <summary>
        /// Parses command line arguments using the available verb types.
        /// </summary>
        /// <param name="assembly">The assembly to scan for verb types.</param>
        /// <param name="verbTypes">The verb types to use for parsing. If null, will scan the assembly.</param>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The parsed command line arguments.</returns>
        public static ParserResult<object> ParseArguments(Assembly assembly, Type[]? verbTypes, string[] args)
        {
            verbTypes ??= GetAvailableVerbTypes(assembly);

            // Skip strict verb matching for special commands like check-deps
            if (!IsCheckDependenciesRequest(args))
            {
                CheckStrictVerbMatching(args, verbTypes);
            }

            return Parser.Default.ParseArguments(args, verbTypes);
        }

        /// <summary>
        /// Determines if the command line arguments represent a check dependencies request.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>True if this is a check dependencies request, false otherwise.</returns>
        public static bool IsCheckDependenciesRequest(string[] args)
        {
            return args.Length == 1 && string.Equals(args[0], "check-deps", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets all available verb types from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>Array of types that have the Verb attribute.</returns>
        private static Type[] GetAvailableVerbTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();
        }

        /// <summary>
        /// Enforce strict verb matching if one verb is marked as default. Otherwise, the default verb will be executed even if that was not the users intention.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="availableVerbTypes">The available verb types.</param>
        /// <exception cref="DotNetConsoleException">Cannot create builder, because the specified verb '{firstVerb}' matches no command.</exception>
        private static void CheckStrictVerbMatching(string[] args, Type[] availableVerbTypes)
        {
            var availableVerbs = availableVerbTypes.Select(t => t.GetCustomAttribute<VerbAttribute>()!).ToList();
            if (!availableVerbs.Any(v => v.IsDefault))
            {
                // If no default verb is defined, we do not enforce strict verb matching
                return;
            }

            var firstVerb = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstVerb) || firstVerb.StartsWith('-'))
            {
                // If the user passed no verb, but a default verb is defined, the default verb will be executed
                return;
            }

            // Names reserved by CommandLineParser library
            var validFirstArguments = new[] { "--help", "--version", "help", "version" }
                .Concat(availableVerbs.Select(t => t.Name))
                .ToList();

            // Check if the first argument can be found in the list of valid arguments
            var verbMatched = validFirstArguments.Any(v => v.Equals(firstVerb, StringComparison.OrdinalIgnoreCase));
            if (!verbMatched)
            {
                throw new DotNetConsoleException($"Cannot create builder, because the specified verb '{firstVerb}' matches no command.");
            }
        }
    }
}
