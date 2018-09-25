// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusMessageStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains data for bus message structure
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains data for bus message structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BusMessageStruct
    {
        /// <summary>
        /// Gets or sets line Id
        /// </summary>
        public ushort LineId;      

        /// <summary>
        /// Umlauf / tour
        /// </summary>
        public ushort Umlauf;      

        /// <summary>
        /// Gets or sets TripId
        /// </summary>
        public ushort TripId;      

        /// <summary>
        /// Gets or sets Ortsnumber
        /// </summary>
        public ushort StopNumber;     

        /// <summary>
        /// Gets or sets wagonId
        /// </summary>
        public ushort WagonId;     

        /// <summary>
        /// distance from start station
        /// </summary>
        public ushort Distance;    

        /// <summary>
        /// min. time to end station
        /// </summary>
        public ushort TminToEndHP; 

        /// <summary>
        /// Gets or sets hopcount
        /// </summary>
        public sbyte Hopcount;      

        /// <summary>
        /// Gets or sets flags
        /// </summary>
        public byte Flags;         

        /// <summary>
        /// Gets or sets validity
        /// </summary>
        public byte Validity; 

        /// <summary>
        /// Gets or sets attribute
        /// </summary>
        public byte Attribute; 

        /// <summary>
        ///  delta time from detector to next Iqube (+/-seconds)
        /// </summary>
        public short DeltaTime;

        /// <summary>
        /// delta distance from detector to next Iqube (+/- meter)
        /// </summary>
        public short DeltaDist;

        /// <summary>
        /// scheduled time to end station
        /// </summary>
        public ushort TimeToEndHP;

        /// <summary>
        /// absolute delay time (+/-seconds)
        /// </summary>
        public short DelayTime;

        /// <summary>
        /// ortsnumber of previous station
        /// </summary>
        public ushort PreviousStop;

        /// <summary>
        /// wait time (seconds)
        /// </summary>
        public ushort WaitTime;
    }
}
