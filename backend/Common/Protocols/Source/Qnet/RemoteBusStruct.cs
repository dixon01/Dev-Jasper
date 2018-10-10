// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteBusStruct.cs" company="Gorba">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure to handle all Qnet messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Protocols.Qnet.Structures;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BusLineWindowDirectRemoteControlStruct
    {
        /// <summary>
        /// Disp info - countdown value
        /// </summary>
        public Int16 DispVal;

        /// <summary>
        /// disposition time in week seconds.
        /// </summary>
        public uint DispTime;

        /// <summary>
        /// Identifier of the Line text (DWORD)
        /// </summary>
        public uint LineId;

        /// <summary>
        /// Identifier of the Lane (DWORD)
        /// </summary>
        public uint LaneId;

        private fixed byte internalLineText[QnetConstantes.LineTextLenght + 1];

        private fixed byte internalDestText1[QnetConstantes.MAX_TEXTLEN_MEDIUM + 1];

        private fixed byte internalDestText2[QnetConstantes.MAX_TEXTLEN_MEDIUM + 1];

        private fixed byte internalLaneText[QnetConstantes.LineTextLenght + 1];

        private fixed byte internalTimeText[QnetConstantes.LineTextLenght + 1];

        /// <summary>
        /// Gets or sets the Text 1 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        public string LineText
        {
            get
            {
                fixed (byte* ptr = this.internalLineText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, QnetConstantes.LineTextLenght);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalLineText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, QnetConstantes.LineTextLenght);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Destination Text 1 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        public string DestText1
        {
            get
            {
                fixed (byte* ptr = this.internalDestText1)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, QnetConstantes.MAX_TEXTLEN_MEDIUM);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalDestText1)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, QnetConstantes.MAX_TEXTLEN_MEDIUM);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Destination Text 2 with up to 32 characters (filled with trailing zeros!)
        /// </summary>
        public string DestText2
        {
            get
            {
                fixed (byte* ptr = this.internalDestText2)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, QnetConstantes.MAX_TEXTLEN_MEDIUM);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalDestText2)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, QnetConstantes.MAX_TEXTLEN_MEDIUM);
                }
            }
        }

        public string LaneText
        {
            get
            {
                fixed (byte* ptr = this.internalLaneText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, QnetConstantes.LineTextLenght);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalLaneText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, QnetConstantes.LineTextLenght);
                }
            }
        }

        public string TimeText
        {
            get
            {
                fixed (byte* ptr = this.internalTimeText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, QnetConstantes.LineTextLenght);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalTimeText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, QnetConstantes.LineTextLenght);
                }
            }
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RemoteBusStruct
    {
        public ushort msgNumber;

        public ushort cmd;

        public ushort msgIndex;

        public ushort infoFlag;

        public BusLineWindowDirectRemoteControlStruct busLineWindow;
    }

}
