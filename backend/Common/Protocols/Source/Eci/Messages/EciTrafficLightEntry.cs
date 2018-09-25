// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The traffic light message.
    /// </summary>
    public class EciTrafficLightEntry : EciTrafficLightBase
    {
        /// <summary>
        /// Gets the sub type.
        /// </summary>
        public override EciTrafficLightCode SubType
        {
            get
            {
                return EciTrafficLightCode.Entry;
            }
        }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Gets or sets the distance 2.
        /// </summary>
        public double Distance2 { get; set; }

        /// <summary>
        /// Gets or sets the distance 3.
        /// </summary>
        public double Distance3 { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Gets or sets the time 2.
        /// </summary>
        public TimeSpan Time2 { get; set; }

        /// <summary>
        /// Gets or sets the time 3.
        /// </summary>
        public TimeSpan Time3 { get; set; }
    }
}
