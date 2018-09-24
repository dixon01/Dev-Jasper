// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityIdsStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure that defines fields for iqube task.
//   Task is owned by the IqubeCmdMsgStruct
//   <remarks>There is only the Infoline task that is implemented !</remarks>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure that defines fields for iqube task.
    /// Task is owned by the<see cref="IqubeCmdMsgStruct"/>. Legacy code = tDisposition
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActivityIdsStruct
    {
        /// <summary>
        /// Number of activities identifiers stored in the array <see cref="Ids"/>
        /// </summary>
        public byte Count;

        /// <summary>
        /// Contains the header of the task structure. See <see cref="IqubeTaskHdrStruct"/> for details.
        /// </summary>
        public fixed uint Ids[MessageConstantes.MaxActivitiesId];       
    }
} 