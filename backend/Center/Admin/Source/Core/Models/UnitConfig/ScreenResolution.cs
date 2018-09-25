// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenResolution.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenResolution type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The screen resolution.
    /// </summary>
    public class ScreenResolution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenResolution"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the resolution.
        /// </param>
        /// <param name="visibleWidth">
        /// The width in pixels.
        /// </param>
        /// <param name="visibleHeight">
        /// The height in pixels.
        /// </param>
        public ScreenResolution(string name, int visibleWidth, int visibleHeight)
            : this(name, visibleWidth, visibleHeight, visibleWidth, visibleHeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenResolution"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the resolution.
        /// </param>
        /// <param name="visibleWidth">
        /// The visual width in pixels.
        /// </param>
        /// <param name="visibleHeight">
        /// The visual height in pixels.
        /// </param>
        /// <param name="physicalWidth">
        /// The physical width in pixels.
        /// </param>
        /// <param name="physicalHeight">
        /// The physical height in pixels.
        /// </param>
        public ScreenResolution(string name, int visibleWidth, int visibleHeight, int physicalWidth, int physicalHeight)
        {
            this.Name = name;
            this.VisibleWidth = visibleWidth;
            this.VisibleHeight = visibleHeight;
            this.PhysicalWidth = physicalWidth;
            this.PhysicalHeight = physicalHeight;
        }

        /// <summary>
        /// Gets the name of the resolution.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the visible width in pixels.
        /// </summary>
        public int VisibleWidth { get; private set; }

        /// <summary>
        /// Gets the visible height in pixels.
        /// </summary>
        public int VisibleHeight { get; private set; }

        /// <summary>
        /// Gets the physical width in pixels.
        /// </summary>
        public int PhysicalWidth { get; private set; }

        /// <summary>
        /// Gets the physical height in pixels.
        /// </summary>
        public int PhysicalHeight { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                AdminStrings.UnitConfig_Hardware_ScreenResolutions_Format,
                this.Name,
                this.VisibleWidth,
                this.VisibleHeight);
        }
    }
}