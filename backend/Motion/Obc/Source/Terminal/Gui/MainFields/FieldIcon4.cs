// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldIcon4.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldIcon4 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    ///   Represent the image field 4. This class is used because on this picture box
    ///   multiple icons are used. This class knows which icon to show there...
    ///   Handles the Detour/Redirection and Traffic Light icons
    /// </summary>
    internal class FieldIcon4
    {
        private const int PosDetour = 0;
        private const int PosRequested = 1;
        private const int PosReceived = 2;
        private readonly ImageList imageList;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIcon4"/> class.
        /// </summary>
        /// <param name="imageList">
        /// The image list.
        /// </param>
        public FieldIcon4(ImageList imageList)
        {
            this.imageList = imageList;
            if (imageList.Images.Count != 3)
            {
                throw new ArgumentException("FieldIcon4.cs Constructor: ImageList has to have exact 3 images!");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bus is on a detour.
        /// </summary>
        public bool Detour { get; set; }

        /// <summary>
        /// Gets or sets the traffic light status.
        /// </summary>
        public TrafficLightIconState TrafficLight { get; set; }

        /// <summary>
        ///   When no image has to be displayed, return value is null.
        /// </summary>
        /// <returns>check if NULL. When yes, hide the image box</returns>
        public Image GetImage()
        {
            if (this.TrafficLight == TrafficLightIconState.Received)
            {
                return this.imageList.Images[PosReceived];
            }

            if (this.TrafficLight == TrafficLightIconState.Requested)
            {
                return this.imageList.Images[PosRequested];
            }

            if (this.Detour)
            {
                return this.imageList.Images[PosDetour];
            }

            return null;
        }
    }
}