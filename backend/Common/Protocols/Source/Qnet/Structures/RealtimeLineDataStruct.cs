// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeLineDataStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Realtime monitoring line data for led matrix display (type M and L)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Realtime monitoring line data for led matrix display (type M and L)
    /// </summary>
    /// <remarks>Len = 10 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RealtimeLineDataStruct
    {
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
        /// Text of line
        /// </summary>
        private fixed byte internalText[MessageConstantes.MaxLedMatrixDisplayLineTextLenght];

        /// <summary>
        /// Gets or sets the text corresponding to the displayed line.
        /// </summary>
        public string Text
        {
            get
            {
                fixed (byte* ptr = this.internalText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(
                        ptr, MessageConstantes.MaxLedMatrixDisplayLineTextLenght);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(
                        value, ptr, MessageConstantes.MaxLedMatrixDisplayLineTextLenght);
                }
            }
        }
    }
}
