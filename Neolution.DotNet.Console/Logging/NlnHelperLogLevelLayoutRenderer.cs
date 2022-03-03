namespace Neolution.DotNet.Console.Logging
{
    using System;
    using System.Text;
    using NLog;
    using NLog.LayoutRenderers;

    /// <inheritdoc />
    /// <summary>
    /// Custom NLog layout renderer to render log level like NLNHelper did.
    /// </summary>
    /// <seealso cref="LayoutRenderer" />
    [NLog.LayoutRenderers.LayoutRenderer("NLNHelperLogLevel")]
    public class NlnHelperLogLevelLayoutRenderer : LayoutRenderer
    {
        /// <inheritdoc />
        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            // Only print LogLevel when it is Error or higher. Otherwise print spaces instead.
            builder.Append(logEvent.Level >= LogLevel.Error ? $"[{logEvent.Level}]" : "       ");
        }
    }
}
