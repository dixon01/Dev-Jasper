// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Context for <see cref="IUpdateClient"/> and <see cref="IUpdateProvider"/>.
    /// </summary>
    public interface IUpdateContext
    {
        /// <summary>
        /// Gets the resource provider to be used for resource handling.
        /// </summary>
        IResourceProvider ResourceProvider { get; }

        /// <summary>
        /// Gets the temporary directory where data about the update can be stored.
        /// </summary>
        string TemporaryDirectory { get; }

        /// <summary>
        /// Gets all the registered update sinks.
        /// </summary>
        IEnumerable<IUpdateSink> Sinks { get; }

        /// <summary>
        /// Creates a new progress monitor for the given state.
        /// </summary>
        /// <param name="stage">
        /// The update stage for which the progress is shown.
        /// </param>
        /// <param name="showVisualization">
        /// Show Visualization if true for every Client, Provider and agents
        /// </param>
        /// <returns>
        /// The <see cref="IProgressMonitor"/>; never null.
        /// </returns>
        IProgressMonitor CreateProgressMonitor(UpdateStage stage, bool showVisualization);
    }
}
