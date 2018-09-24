// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="VolumeSettingsMessageResponse.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    /// <summary>The volume settings message response.</summary>
    [Serializable]
    public class AudioStatusResponse : VolumeSettingsMessage
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AudioStatusResponse"/> class.</summary>
        /// <param name="config">The config.</param>
        public AudioStatusResponse(VolumeSettingsMessage config)
        {
            this.Interior = config.Interior;
            this.Exterior = config.Exterior;
            this.RefreshIntervalMiliSeconds = config.RefreshIntervalMiliSeconds;
            this.MessageAction = config.MessageAction;
        }

        /// <summary>Initializes a new instance of the <see cref="AudioStatusResponse" /> class.</summary>
        public AudioStatusResponse()
            : base()
        {
        }

        #endregion
    }
}