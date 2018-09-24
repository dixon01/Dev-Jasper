// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStruct.cs" company="Gorba AG">
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
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActivityStruct
    {
        /// <summary>
        /// Contains the header of the task structure. See <see cref="IqubeTaskHdrStruct"/> for details.
        /// </summary>
        [FieldOffset(0)]
        public IqubeTaskHdrStruct Header;

        /// <summary>
        /// Contains the Special info line text data.
        /// Len = 171
        /// </summary>
        [FieldOffset(15)]
        public InfoLineStruct InfoLine;

        /// <summary>
        /// Contains the Special info line text data
        /// Len = 163 bytes
        /// </summary>
        [FieldOffset(15)]
        public VoiceTextStruct VoiceText;

        /// <summary>
        /// display data, (legacy code = tDisplay)  - used for TASK_DISPO_DISP_OFF, TASK_DISPO_DISP_ON
        /// Len = 150 bytes
        /// </summary>
        [FieldOffset(15)]
        public DisplayStruct Display;

        /// <summary>
        /// Enable to delete trip(s) by Itcs provider id, by line id, by direction id or by trip identifier or by a combination of ones of them.
        /// </summary>
        [FieldOffset(15)]
        public DeleteTripStruct DeleteTrip;

        /*union
        {
          tInfoLine   infoLine;
          tDispo      dispo;
          tDisplay    display;
          tQarchive   qarchive;
          tEvtlog     evtlog;
          tUpgrade    upgrade;
          tBatchJob   batchJob;
          tSpecText   specText;
          tVoiceText  voiceText;
        } u;
         * */
    }
}
