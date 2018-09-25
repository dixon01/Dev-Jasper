// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeStartMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates the 2 available Realtime start mode
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumerates the 2 available Realtime start mode
    /// </summary>
    [DataContract]
    public enum RealtimeStartMode
    {
        /// <summary>
        /// Indicates to the iqube to send data only on request.
        /// </summary>
        [EnumMember]
        SendOnDataRequest = 0,

        /// <summary>
        /// Indicates to the iqube to send data and request but also when something change.
        /// </summary>
        [EnumMember]
        SendDataOnChange = 1

    }
}
