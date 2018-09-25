// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="VolumeSettings.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;

    /// <summary>The volume settings.</summary>
    [Serializable]
    public class VolumeSettings : IVolumeSettings
    {
        #region Constructors and Destructors

        public const int DefaultMaxVolume = 100;

        public const int DefaultVolumeLevel = 0;

        /// <summary>Initializes a new instance of the <see cref="VolumeSettings"/> class.</summary>
        /// <param name="currentVolume">The current volume.</param>
        public VolumeSettings(byte currentVolume)
            : this()
        {
            this.CurrentVolume = currentVolume;
        }

        /// <summary>Initializes a new instance of the <see cref="VolumeSettings"/> class.</summary>
        public VolumeSettings()
        {
            // establish default
            this.MinimumVolume = 0;
            this.CurrentVolume = 0;
            this.DefaultVolume = DefaultVolumeLevel;
            this.MaximumVolume = DefaultMaxVolume;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the current volume.</summary>
        [XmlElement]
        public byte CurrentVolume { get; set; }

        /// <summary>Gets or sets the maximum volume.</summary>
        [XmlElement]
        public byte MaximumVolume { get; set; }

        /// <summary>Gets or sets the minimum volume.</summary>
        [XmlElement]
        public byte MinimumVolume { get; set; }

        /// <summary>Gets or sets the default volume.</summary>
        [XmlElement]
        public byte DefaultVolume { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                "Current Volume={0}, Min={1}, Max={2}, Default={3}", 
                this.CurrentVolume, 
                this.MinimumVolume, 
                this.MaximumVolume, 
                this.DefaultVolume);
        }

        #endregion
    }
}