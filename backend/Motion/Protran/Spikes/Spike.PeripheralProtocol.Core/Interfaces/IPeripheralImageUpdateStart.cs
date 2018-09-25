// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralImageUpdateStart.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPeripheralImageUpdateStart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralImageUpdateStart interface.</summary>
    public interface IPeripheralImageUpdateStart : IPeripheralAudioSwtichBaseMessage
    {
        ushort Records { get; set; }
    }

    /// <summary>The PeripheralImageStatus interface.</summary>
    public interface IPeripheralImageStatus : IPeripheralAudioSwtichBaseMessage
    {
        PeripheralImageStatusType Status { get; set; }
    }

    public interface IPeripheralImageUpdateCancel : IPeripheralAudioSwtichBaseMessage
    {
    }
}