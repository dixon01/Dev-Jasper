// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmClass.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates the available values to define the alarm class or alarm severity.
//   <remarks>Correponds to tALARMclass from the legacy of qnet.</remarks>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumerates the available values to define the alarm class or alarm severity. 
    /// <remarks>Correponds to tALARMclass from the legacy of qnet.</remarks>
    /// </summary>
    [DataContract]
    public enum AlarmClass
    {
        /// <summary>
        /// No class defined
        /// </summary>
        [EnumMember]
        None = -1,

        /// <summary>
        /// Type information
        /// </summary>
        [EnumMember]
        Info = 0,

        /// <summary>
        /// Type Warning
        /// </summary>
        [EnumMember]
        Warning,

        /// <summary>
        /// Minor alarm
        /// </summary>
        [EnumMember]
        Minor,

        /// <summary>
        /// Major alarm
        /// </summary>
        [EnumMember]
        Major,

        /// <summary>
        /// High alarm
        /// </summary>
        [EnumMember]
        High
    }
} 
