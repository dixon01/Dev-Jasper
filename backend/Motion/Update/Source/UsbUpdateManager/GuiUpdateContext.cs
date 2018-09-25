// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuiUpdateContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GuiUpdateContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.Medi;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Implementation of <see cref="IUpdateContext"/> for use in the USB Update Manager.
    /// </summary>
    public class GuiUpdateContext : IUpdateContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuiUpdateContext"/> class.
        /// </summary>
        /// <param name="resourceService">
        /// The Medi resource service.
        /// </param>
        public GuiUpdateContext(IResourceService resourceService)
        {
            this.ResourceProvider = new MediResourceProvider(resourceService);
            this.Sinks = new IUpdateSink[0];
        }

        /// <summary>
        /// Gets the resource provider to be used for resource handling.
        /// </summary>
        public IResourceProvider ResourceProvider { get; private set; }

        /// <summary>
        /// Gets the temporary directory where data about the update can be stored.
        /// </summary>
        public string TemporaryDirectory
        {
            get
            {
                return Path.GetTempPath();
            }
        }

        /// <summary>
        /// Gets all the registered update sinks.
        /// </summary>
        public IEnumerable<IUpdateSink> Sinks { get; private set; }

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
        public IProgressMonitor CreateProgressMonitor(UpdateStage stage, bool showVisualization)
        {
            return new NullProgressMonitor();
        }
    }
}
