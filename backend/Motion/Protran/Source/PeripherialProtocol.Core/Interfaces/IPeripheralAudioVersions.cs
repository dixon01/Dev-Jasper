// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralAudioVersions.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>The PeripheralAudioVersions interface.</summary>
    public interface IPeripheralAudioVersions : IPeripheralBaseMessage
    {
        /// <summary>Gets or sets the versions info.</summary>
        PeripheralVersionsInfo VersionsInfo { get; set; }

        string HardwareVersion { get; }

        string SerialNumber { get; }

        string SoftwareVersion { get; }
    }
}