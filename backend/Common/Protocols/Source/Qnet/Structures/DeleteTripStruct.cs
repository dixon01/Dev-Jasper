// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteTripStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines DeleteTripStruct used with <see cref="ActivityStruct" />. 
//   Len = 12 bytes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines DeleteTripStruct used with <see cref="ActivityStruct" />. Enables to delete trip from timetable according to the filter 
    /// set by the ItcsProvider, LineId, DirectionId and TripId or a combintation of ones of them. Each part of filter could be set to 0.
    /// Len = 12 bytes
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DeleteTripStruct
    {
        /// <summary>
        /// ITCS provider identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </summary>
        public ushort ItcsProviderId;

        /// <summary>
        /// Line identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </summary>
        public uint LineId;

        /// <summary>
        /// Direction identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </summary>
        public ushort DirectionId;

        /// <summary>
        /// Trip identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </summary>
        public uint TripId;
    }
}
