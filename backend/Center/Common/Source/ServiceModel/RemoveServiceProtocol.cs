// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveServiceProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoveServiceProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the available data services protocol.
    /// </summary>
    [DataContract]
    public enum RemoveServiceProtocol
    {
        /// <summary>
        /// The Http protocol.
        /// </summary>
        [EnumMember]
        Http = 0,

        /// <summary>
        /// The Tcp protocol.
        /// </summary>
        [EnumMember]
        Tcp = 1
    }
}