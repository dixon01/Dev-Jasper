// -----------------------------------------------------------------------
// <copyright file="RealtimeInfoLineStruct.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <remarks>Len = 164 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RealtimeInfoLineStruct
    {
        /// <summary>
        /// Number of the row on display because could have more than one info line displayed.
        /// </summary>
        public byte RowNumber;

        /// <summary>
        /// Contains info on the 
        /// Bitcoded: 0x1-0x2:Alignment; 0x4:Blink; 0x8:Scroll; 0x10:AutoInfoline; 0x80:Valid
        /// </summary>
        public byte Attributes;

        /// <summary>
        /// Font of the info line. Unused
        /// </summary>
        public sbyte Font;

        /// <summary>
        /// Text of voice text
        /// </summary>
        private fixed byte internalText[MessageConstantes.MaxInfoLineTextLenght];

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
                        ptr, MessageConstantes.MaxInfoLineTextLenght);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(
                        value, ptr, MessageConstantes.MaxInfoLineTextLenght);
                }
            }
        }
    }
}
