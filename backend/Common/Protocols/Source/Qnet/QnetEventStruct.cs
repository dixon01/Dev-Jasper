// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetEventStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains data from qnet alarm message sent by an iqube.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains data from qnet alarm message sent by an iqube.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct QnetEventStruct
    {
        /// <summary>
        /// Identifier of the event
        /// </summary>
        public short EventId;

        /// <summary>
        /// Event time stamp
        /// </summary>
        public DOSTimeStruct Time;

        /// <summary>
        /// event attribute (legacy code = attrib)
        /// </summary>
        public byte Attribute;     

        /// <summary>
        /// event flags
        /// </summary>
        public byte Flags;     

        /// <summary>
        /// Event alarm class
        /// </summary>
        public byte AlarmClass;

        /// <summary>
        /// Event parameter number 0
        /// </summary>
        public byte Param;

        /// <summary>
        /// Event parameter number 1
        /// </summary>
        public ushort Param1;

        /// <summary>
        /// Event parameter number 2
        /// </summary>
        public ushort Param2;

        /// <summary>
        /// Event parameter number 3
        /// </summary>
        public ushort Param3;

        /// <summary>
        /// Contains additional data for some events
        /// </summary>
        public fixed byte AdditionalData[MessageConstantes.EventDataLength];
    }
}
