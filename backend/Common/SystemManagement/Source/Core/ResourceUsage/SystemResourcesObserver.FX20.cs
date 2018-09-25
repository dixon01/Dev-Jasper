// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemResourcesObserver.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using Microsoft.VisualBasic.Devices;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of the system and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class SystemResourcesObserver
    {
        private readonly ComputerInfo computerInfo = new ComputerInfo();

        /// <summary>
        /// Gets the currently available RAM in the system.
        /// </summary>
        public long AvailableRam
        {
            get
            {
                return (long)this.computerInfo.AvailablePhysicalMemory;
            }
        }

        /// <summary>
        /// Gets the total amount of RAM installed in the system.
        /// </summary>
        public long TotalRam
        {
            get
            {
                return (long)this.computerInfo.TotalPhysicalMemory;
            }
        }
    }
}