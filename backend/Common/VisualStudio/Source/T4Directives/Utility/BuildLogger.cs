// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildLogger.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BuildLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.Utility
{
    using System.Text;

    using Microsoft.Build.Framework;

    /// <summary>
    /// Component used to log the output of a build.
    /// </summary>
    internal class BuildLogger : ILogger
    {
        private readonly StringBuilder output = new StringBuilder();

        /// <summary>
        /// Gets or sets the level of detail to show in the event log.
        /// </summary>
        /// <returns>
        /// One of the enumeration values. The default is
        /// <see cref="F:Microsoft.Build.Framework.LoggerVerbosity.Normal"/>.
        /// </returns>
        public LoggerVerbosity Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the user-defined parameters of the logger.
        /// </summary>
        /// <returns>
        /// The logger parameters.
        /// </returns>
        public string Parameters { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="eventSource">
        /// The event source.
        /// </param>
        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += this.EventSourceOnErrorRaised;
            eventSource.BuildFinished += this.EventSourceOnBuildFinished;
        }

        /// <summary>
        /// Releases the resources allocated to the logger at the time of initialization or during the build. This
        /// method is called when the logger is unregistered from the engine, after all events are raised. A host of
        /// MSBuild typically unregisters loggers immediately before quitting.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// Gets the output of the logger.
        /// </summary>
        /// <returns>The output of the logger.</returns>
        public string GetOutput()
        {
            return this.output.ToString();
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs buildErrorEventArgs)
        {
            this.output.AppendFormat("Error: {0}", buildErrorEventArgs.Message);
        }

        private void EventSourceOnBuildFinished(object sender, BuildFinishedEventArgs buildFinishedEventArgs)
        {
            this.output.AppendFormat("Info: Build completed. Succeeded: {0}", buildFinishedEventArgs.Succeeded);
        }
    }
}
