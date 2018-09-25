// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralAudioConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    /// <summary>The PeripheralAudioConfig interface.</summary>
    public interface IPeripheralAudioConfig : IPeripheralBaseMessage
    {
        #region Public Properties

        /// <summary>Gets or sets the exterior default volume.</summary>
        byte ExteriorDefaultVolume { get; set; }

        /// <summary>Gets or sets the exterior max allowed volume.</summary>
        byte ExteriorMaxAllowedVolume { get; set; }

        /// <summary>Gets or sets the exterior max volume.</summary>
        byte ExteriorMaxVolume { get; set; }

        /// <summary>Gets or sets the exterior min volume.</summary>
        byte ExteriorMinVolume { get; set; }

        /// <summary>Gets or sets the default volume.</summary>
        byte InteriorDefaultVolume { get; set; }

        /// <summary>Gets or sets the interior max allowed volume.</summary>
        byte InteriorMaxAllowedVolume { get; set; }

        /// <summary>Gets or sets the interior max volume.</summary>
        byte InteriorMaxVolume { get; set; }

        /// <summary>Gets or sets the interior min volume.</summary>
        byte InteriorMinVolume { get; set; }

        /// <summary>Gets or sets the line in enable.</summary>
        byte LineInEnable { get; set; }

        /// <summary>Gets or sets the noise level.</summary>
        byte NoiseLevel { get; set; }

        /// <summary>Gets or sets the push to talk lockout time.</summary>
        ushort PushToTalkLockoutTime { get; set; }

        /// <summary>Gets or sets the push to talk timeout.</summary>
        ushort PushToTalkTimeout { get; set; }

        #endregion
    }
}