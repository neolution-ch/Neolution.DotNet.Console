namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    /// <summary>
    /// The fake main console.
    /// </summary>
    public class MainConsoleFake
    {
        /// <summary>
        /// The result
        /// </summary>
        private readonly string result = "Run";

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>The result as string.</returns>
        public string Run()
        {
            return this.result;
        }
    }
}
