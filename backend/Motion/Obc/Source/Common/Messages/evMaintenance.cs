// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evMaintenance.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evMaintenance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The maintenance event.
    /// </summary>
    public class evMaintenance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evMaintenance"/> class.
        /// </summary>
        public evMaintenance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evMaintenance"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public evMaintenance(Types type)
        {
            this.Type = type;
        }

        /// <summary>
        /// The possible types.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// The initialize duty.
            /// </summary>
            InitDuty,

            /// <summary>
            /// The initialize alarm.
            /// </summary>
            InitAlarm
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Types Type { get; set; }
    }
}