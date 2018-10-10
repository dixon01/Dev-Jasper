// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessResourcesObserver.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;

    using NLog;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of a process and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class ProcessResourcesObserver
    {
        /// <summary>
        /// Gets the used RAM (working set) in bytes.
        /// </summary>
        public long RamBytes
        {
            get
            {
                try
                {
                    return this.process.WorkingSet64;
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Couldn't get working set size");
                    return 0;
                }
            }
        }
    }
}