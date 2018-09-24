// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActiveSpeakerType.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;

    /// <summary>The active speaker zone type.</summary>
    [Flags]
    public enum ActiveSpeakerZone : byte
    {
        /// <summary>The none.</summary>
        None = 0, 

        /// <summary>The interior.</summary>
        Interior = 0x1, 

        /// <summary>The exterior.</summary>
        Exterior = 0x2, 

        /// <summary>The both.</summary>
        Both = Interior | Exterior
    }
}