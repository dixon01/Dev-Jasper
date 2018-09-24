// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DOSTimeStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains data reprensenting the date time in DOS format
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains data reprensenting the date time in DOS format.
    /// Size = 4 bytes
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DOSTimeStruct
    {
        // bugtracker #221 : change time and date from short to unsigned short

        /// <summary>
        /// Time in DOS form: HHHHH(32) MMMMMM(64) SSSSS(32)
        /// </summary>
        public ushort Time;

        /// <summary>
        /// Date in DOS form: YYYYYYY(128) MMMM(16) DDDDD(32)
        /// </summary>
        public ushort Date;
    }
}
