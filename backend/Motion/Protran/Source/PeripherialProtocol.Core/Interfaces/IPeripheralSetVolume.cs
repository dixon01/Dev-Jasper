// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralSetVolume.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    /// <summary>The PeripheralSetVolume interface.</summary>
    public interface IPeripheralSetVolume : IPeripheralBaseMessage
    {
        #region Public Properties

        /// <summary>Gets or sets the exterior volume.</summary>
        byte ExteriorVolume { get; set; }

        /// <summary>Gets or sets the interior volume.</summary>
        byte InteriorVolume { get; set; }

        #endregion
    }
}