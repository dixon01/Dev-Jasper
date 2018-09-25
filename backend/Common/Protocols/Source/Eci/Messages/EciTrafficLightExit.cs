// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightExit.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The traffic light exit.
    /// </summary>
    public class EciTrafficLightExit : EciTrafficLightBase
    {
        /// <summary>
        /// Gets the sub type.
        /// </summary>
        public override EciTrafficLightCode SubType
        {
            get
            {
                return EciTrafficLightCode.Exit;
            }
        }
    }
}
