// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightCheckPoint.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The traffic light check point.
    /// </summary>
    public class EciTrafficLightCheckPoint : EciTrafficLightBase
    {
        /// <summary>
        /// Gets the sub type.
        /// </summary>
        public override EciTrafficLightCode SubType
        {
            get
            {
                return EciTrafficLightCode.Checkpoint;
            }
        }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}
