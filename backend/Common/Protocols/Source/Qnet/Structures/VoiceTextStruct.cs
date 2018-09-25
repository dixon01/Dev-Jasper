// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoiceTextStruct.cs" company="Gorba AG">
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

    // using Gorba.Common.Protocols.Qnet;

    /// <summary>
    /// Structure that defines fields for voice text task.
    /// Task is owned by the <see cref="IqubeCmdMsgStruct"/>
    /// Len = 163 bytes
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VoiceTextStruct
    {
        /// <summary>
        /// Text of voice text
        /// </summary>
        private fixed byte internalText[MessageConstantes.MaxVoiceTextLength];

        /// <summary>
        /// Gets or sets the text corresponding to the lane.
        /// </summary>
        public string Text
        {
            get
            {
                fixed (byte* ptr = this.internalText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(
                        ptr, MessageConstantes.MaxVoiceTextLength);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(
                        value, ptr, MessageConstantes.MaxVoiceTextLength);
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval to repeat voice text [in seconds], can be zero.
        /// </summary>
        public ushort Interval { get; set; }
    }
}
