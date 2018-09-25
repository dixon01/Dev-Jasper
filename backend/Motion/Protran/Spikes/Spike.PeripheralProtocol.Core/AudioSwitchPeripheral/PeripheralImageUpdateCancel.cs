// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageUpdateCancel.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    #region Notes
    // IMAGE LOAD CANCEL

    // typedef struct _g3_imagecancel_t
    //{
    //    PCP_HEADER hdr;
    //    uint_8 chksum;
    //} PACKED(PCP_07_IMAGE_CANCEL_PKT);

    #endregion

    /// <summary>The peripheral image update cancel.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Size)]
    public class PeripheralImageUpdateCancel : PeripheralAudioSwtichBaseMessage, IPeripheralRequestVersion<PeripheralAudioSwitchMessageType>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PeripheralImageUpdateCancel" /> class. Initializes a new instance
        ///     of the <see cref="ImageUpdateCancel" /> class.
        /// </summary>
        public PeripheralImageUpdateCancel()
            : base(PeripheralAudioSwitchMessageType.ImageUpdateCancel)
        {
        }        
    }
}