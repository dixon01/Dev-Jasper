// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDisplayType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates all RealtimeDisplay types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumerates all RealtimeDisplay types.
    /// </summary>
    [DataContract]
    public enum RealtimeDisplayType
    {
        /// <summary>
        ///  display type "Infoline". (legacy code = RTMON_DISPLAY_TYP_INFO)
        /// </summary>
        [EnumMember]
        InfoLine = 0,

        /// <summary>
        /// display type "Standard Display". (legacy code = RTMON_DISPLAY_TYP_S)
        /// </summary>
        [EnumMember]
        DisplayTypeS = 1,

        /// <summary>
        /// display type "Led Countdown Display". (legacy code = RTMON_DISPLAY_TYP_C)
        /// </summary>
        [EnumMember]
        DisplayTypeC = 2,

        /// <summary>
        /// display type "Led Matrix Display M" - without Lane information. (legacy code = RTMON_DISPLAY_TYP_M)
        /// </summary>
        [EnumMember]
        DisplayTypeM = 3,

        /// <summary>
        /// display type "Led Matrix Display L" - with Lane information. (legacy code = RTMON_DISPLAY_TYP_L)
        /// </summary>
        [EnumMember]
        DisplayTypeL = 4
    }
}
