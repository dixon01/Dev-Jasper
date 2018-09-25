// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageUpdateCancel.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    // IMAGE LOAD CANCEL
    //typedef struct _g3_imagecancel_t
    //{
    //    PCP_HEADER hdr;
    //    uint_8 chksum;
    //} PACKED(PCP_07_IMAGE_CANCEL_PKT);


    /// <summary>The peripheral image update cancel.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralImageUpdateCancel : PeripheralBaseMessage
    {
        /// <summary>The size.</summary>
        public new const int Size = PeripheralBaseMessage.Size;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PeripheralImageUpdateCancel" /> class. Initializes a new instance
        ///     of the <see cref="ImageUpdateCancel" /> class.
        /// </summary>
        public PeripheralImageUpdateCancel()
            : base(PeripheralMessageType.ImageUpdateCancel)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateCancel"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system Message Type.</param>
        public PeripheralImageUpdateCancel(
            ushort address, 
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.AudioGeneration3)
            : base(PeripheralMessageType.ImageUpdateCancel, systemMessageType)
        {
            this.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}