// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemResourcesObserver.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using OpenNETCF.WindowsCE;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of the system and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class SystemResourcesObserver
    {
        /// <summary>
        /// Gets the currently available RAM in the system.
        /// </summary>
        public long AvailableRam
        {
            get
            {
                return MemoryManagement.AvailablePhysicalMemory;
            }
        }

        /// <summary>
        /// Gets the total amount of RAM installed in the system.
        /// </summary>
        public long TotalRam
        {
            get
            {
                return MemoryManagement.TotalPhysicalMemory;
            }
        }
    }
}