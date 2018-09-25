// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SectionType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Configuration
{
    /// <summary>
    /// Defines the section types for the configuration.
    /// </summary>
    internal enum SectionType
    {
        /// <summary>
        /// The memory slots.
        /// </summary>
        MemorySlots = 0x10,

        /// <summary>
        /// The other files.
        /// </summary>
        OtherFiles = 0x11,

        /// <summary>
        /// The scheduler table.
        /// </summary>
        SchedulerTable = 0x20,

        /// <summary>
        /// The configuration options.
        /// </summary>
        ConfigurationOptions = 0x30
    }
}