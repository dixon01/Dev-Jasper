// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="CannedPlaybackMessage.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <summary>
//   Canned Audio/Video playback message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    /// <summary>The canned message.</summary>
    [Serializable]
    public class CannedPlaybackMessage
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CannedPlaybackMessage"/> class.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="audioZoneTypes">The canned message zone type.</param>
        /// <param name="cannedMessageEncodingType">The canned message encoding type.</param>
        public CannedPlaybackMessage(string fileName, AudioZoneTypes audioZoneTypes, CannedMessageEncodingType cannedMessageEncodingType)
        {
            this.FileName = fileName;
            this.AudioZoneTypes = audioZoneTypes;
            this.CannedMessageEncodingType = cannedMessageEncodingType;
        }

        /// <summary>Initializes a new instance of the <see cref="CannedPlaybackMessage"/> class.</summary>
        public CannedPlaybackMessage()
            : this(string.Empty, AudioZoneTypes.None, CannedMessageEncodingType.Unknown)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the canned message encoding type.</summary>
        public CannedMessageEncodingType CannedMessageEncodingType { get; set; }

        /// <summary>Gets or sets the canned message zone type.</summary>
        public AudioZoneTypes AudioZoneTypes { get; set; }

        /// <summary>Gets or sets the file name.</summary>
        public string FileName { get; set; }

        /// <summary>Gets or sets the message id.</summary>
        public string MessageId { get; set; }

        public override string ToString()
        {
            return $"File={this.FileName}, MessageId={this.MessageId}, AudioZone={this.AudioZoneTypes}, Type={this.CannedMessageEncodingType}";
        }

        #endregion
    }
}