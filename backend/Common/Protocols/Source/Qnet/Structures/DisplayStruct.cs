// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure that defines fields for DisplayStruct.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure that defines fields for DisplayStruct.
    /// Len = 38 + (8*2) + (2 * 48) = 150
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DisplayStruct
    {
        /// <summary>
        /// See <see cref="ActivityDataStruct"/>- Only LineId and wParam1 are used, set all others to Zero [0]
        /// </summary>
        public ActivityDataStruct ActivityData;

        /// <summary>
        /// Not used
        /// len = 8*2 = 16
        /// </summary>
        public fixed ushort Lines[MessageConstantes.MaxMessageLines];

        /// <summary>
        /// Text 1 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        private fixed byte internalText1[MessageConstantes.MaxActivitiesId];

        /// <summary>
        /// Text 2 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        private fixed byte internalText2[MessageConstantes.MaxActivitiesId];

        /// <summary>
        /// Gets or sets the Text 1 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        public string Text1
        {
            get
            {
                fixed (byte* ptr = this.internalText1)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, MessageConstantes.MaxActivitiesId);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText1)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, MessageConstantes.MaxActivitiesId);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Text 2 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        public string Text2
        {
            get
            {
                fixed (byte* ptr = this.internalText2)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, MessageConstantes.MaxActivitiesId);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText2)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, MessageConstantes.MaxActivitiesId);
                }
            }
        }
    }
}
