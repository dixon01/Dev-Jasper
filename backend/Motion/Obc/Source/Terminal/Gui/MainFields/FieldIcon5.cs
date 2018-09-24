// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldIcon5.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldIcon5 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    ///   Represent the image field 1. This class is used because on this picture box
    ///   multiple icons are used. This class knows which icon to show there...
    /// </summary>
    internal class FieldIcon5 // : IGetImage
    {
        private const int ImgSpeechConnecting = 0;

        private const int ImgSpeechConnected = 1;

        private readonly ImageList imageList;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIcon5"/> class.
        /// </summary>
        /// <param name="imageList">
        /// The image list.
        /// </param>
        public FieldIcon5(ImageList imageList)
        {
            this.imageList = imageList;
            if (imageList.Images.Count != 2)
            {
                throw new ArgumentException("FieldIcon5.cs Constructor: ImageList has to have exact 4 images!");
            }
        }

        /// <summary>
        /// Gets or sets the voice state.
        /// </summary>
        public VoiceIconState Voice { get; set; }

        /// <summary>
        ///   When no image has to be displayed, return value is null.
        /// </summary>
        /// <returns>check if NULL. When yes, hide the image box</returns>
        public Image GetImage()
        {
            // speech connected has higher priority than connecting
            if (this.Voice == VoiceIconState.Connected)
            {
                return this.imageList.Images[ImgSpeechConnected];
            }

            if (this.Voice == VoiceIconState.Requested)
            {
                return this.imageList.Images[ImgSpeechConnecting];
            }

            return null;
        }
    }
}