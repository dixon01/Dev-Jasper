// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressManager.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System;

    /// <summary>
    /// Manager that monitors the progress of the update process and informs
    /// <see cref="IUpdateVisualization"/>s about the progress.
    /// </summary>
    public partial class UpdateProgressManager
    {
        partial void CreatePlatformVisualizations()
        {
            if (this.config.Led.Enabled)
            {
                try
                {
                    this.visualizations.Add(new LedUpdateVisualization(this.config.Led));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't create LED update visualization");
                }
            }
        }
    }
}
