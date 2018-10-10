// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="VolumeSettingsMessage.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>The volume settings request.</summary>
    [Serializable]
    public class VolumeSettingsMessage
    {
        #region Constants

        /// <summary>The default audio status poll refresh interval.</summary>
        public const ushort DefaultRefreshInterval = 1000;

        /// <summary>The disable refresh interval value.</summary>
        public const ushort DisabledRefreshInterval = 0;

        /// <summary>The ignored value to use to default the byte property to. Zero is a valid property value.</summary>
        public const byte Ignored = 0xFF;

        #endregion

        #region Fields

        private ushort refreshIntervalMiliSeconds;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="VolumeSettingsMessage" /> class.</summary>
        public VolumeSettingsMessage()
        {
            this.Interior = new VolumeSettings();
            this.Exterior = new VolumeSettings();
            this.MessageAction = MessageActions.None;
            this.RefreshIntervalMiliSeconds = DefaultRefreshInterval;
        }

        /// <summary>Initializes a new instance of the <see cref="VolumeSettingsMessage"/> class.</summary>
        /// <param name="action">The action.</param>
        public VolumeSettingsMessage(MessageActions action)
            : this()
        {
            this.MessageAction = action;
        }

        /// <summary>Initializes a new instance of the <see cref="VolumeSettingsMessage"/> class. Initializes a new instance of
        ///     the <see cref="VolumeSettingsMessage"/> class.</summary>
        /// <param name="messageActions">The message Action.</param>
        /// <param name="interiorHardwareSettings">The interior volumes.</param>
        /// <param name="exteriorHardwareSettings">The exterior volumes.</param>
        /// <param name="refreshIntervalMiliSeconds">The refresh Interval Mili Seconds.</param>
        public VolumeSettingsMessage(
            MessageActions messageActions, 
            VolumeSettings interiorHardwareSettings, 
            VolumeSettings exteriorHardwareSettings, 
            ushort refreshIntervalMiliSeconds)
        {
            this.Interior = interiorHardwareSettings;
            this.Exterior = exteriorHardwareSettings;
            this.MessageAction = messageActions;
            this.RefreshIntervalMiliSeconds = refreshIntervalMiliSeconds;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the refresh interval mili seconds.</summary>
        [XmlElement]
        public ushort RefreshIntervalMiliSeconds
        {
            get
            {
                return this.refreshIntervalMiliSeconds;
            }

            set
            {
                this.refreshIntervalMiliSeconds = value >= 0 ? value : DisabledRefreshInterval;
            }
        }

        /// <summary>Gets or sets the interior.</summary>
        [XmlElement]
        public VolumeSettings Interior { get; set; }

        /// <summary>Gets or sets the exterior.</summary>
        [XmlElement]
        public VolumeSettings Exterior { get; set; }

        /// <summary>Gets or sets a value indicating the application of the message settings.</summary>
        [XmlIgnore]
        public MessageActions MessageAction { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format(
                "Volume settings: Interior = {0}, Exterior = {1}, RefreshIntervalMiliSeconds = {2}", 
                this.Interior, 
                this.Exterior, 
                this.RefreshIntervalMiliSeconds);
        }

        #endregion
    }
}