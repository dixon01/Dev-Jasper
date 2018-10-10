// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldIcon2.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldIcon2 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///   Represent the image field 2. This class is used because on this picture box
    ///   multiple icons are used. This class knows which icon to show there...
    /// </summary>
    internal class FieldIcon2
    {
        private const int PosInfo = 0;
        private const int PosAlarm = 1;
        private const int PosInfoAlarm = 2;

        private readonly ImageList imageList;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIcon2"/> class.
        /// </summary>
        /// <param name="imageList">
        /// The image list.
        /// </param>
        public FieldIcon2(ImageList imageList)
        {
            this.imageList = imageList;
            if (imageList.Images.Count != 3)
            {
                throw new ArgumentException("FieldIcon2.cs Constructor: ImageList has to have exact 3 images!");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an info message is available.
        /// </summary>
        public bool InfoMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether alarm message is available.
        /// </summary>
        public bool AlarmMessage { get; set; }

        /// <summary>
        ///   When no image has to be displayed, return value is null.
        /// </summary>
        /// <returns>check if NULL. When yes, hide the image box</returns>
        public Image GetImage()
        {
            if (this.InfoMessage && this.AlarmMessage)
            {
                return this.imageList.Images[PosInfoAlarm];
            }

            if (this.InfoMessage)
            {
                return this.imageList.Images[PosInfo];
            }

            if (this.AlarmMessage)
            {
                return this.imageList.Images[PosAlarm];
            }

            return null;
        }
    }
}