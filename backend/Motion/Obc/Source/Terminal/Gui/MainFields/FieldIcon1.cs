// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldIcon1.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldIcon1 type.
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
    internal class FieldIcon1 // : IGetImage
    {
        private const int ImgDriverAlarmSent = 0;

        private const int ImgDriverAlarmAck = 1;

        private readonly ImageList imageList;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIcon1"/> class.
        /// </summary>
        /// <param name="imageList">
        /// The image list.
        /// </param>
        public FieldIcon1(ImageList imageList)
        {
            this.imageList = imageList;
            if (imageList.Images.Count != 2)
            {
                throw new ArgumentException("FieldIcon1.cs Constructor: ImageList has to have exact 4 images!");
            }
        }

        /// <summary>
        /// Gets or sets the driver alarm state.
        /// </summary>
        public DriverAlarmIconState DriverAlarm { get; set; }

        /// <summary>
        ///   When no image has to be displayed, return value is null.
        /// </summary>
        /// <returns>check if NULL. When yes, hide the image box</returns>
        public Image GetImage()
        {
            if (this.DriverAlarm == DriverAlarmIconState.Acknowledged)
            {
                return this.imageList.Images[ImgDriverAlarmAck];
            }

            if (this.DriverAlarm == DriverAlarmIconState.Sent)
            {
                return this.imageList.Images[ImgDriverAlarmSent];
            }

            return null;
        }
    }
}