// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The descriptor for a physical display.
    /// </summary>
    [Serializable]
    public class DisplayDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDescriptor"/> class.
        /// </summary>
        public DisplayDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDescriptor"/> class.
        /// </summary>
        /// <param name="visibleWidth">
        /// The <see cref="VisibleWidth"/> and <see cref="PhysicalWidth"/>.
        /// </param>
        /// <param name="visibleHeight">
        /// The <see cref="VisibleHeight"/> and <see cref="PhysicalHeight"/>.
        /// </param>
        public DisplayDescriptor(int visibleWidth, int visibleHeight)
            : this(visibleWidth, visibleHeight, visibleWidth, visibleHeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDescriptor"/> class.
        /// </summary>
        /// <param name="visibleWidth">
        /// The <see cref="VisibleWidth"/>.
        /// </param>
        /// <param name="visibleHeight">
        /// The <see cref="VisibleHeight"/>.
        /// </param>
        /// <param name="physicalWidth">
        /// The <see cref="PhysicalWidth"/>.
        /// </param>
        /// <param name="physicalHeight">
        /// The <see cref="PhysicalHeight"/>.
        /// </param>
        public DisplayDescriptor(int visibleWidth, int visibleHeight, int physicalWidth, int physicalHeight)
        {
            this.VisibleWidth = visibleWidth;
            this.VisibleHeight = visibleHeight;
            this.PhysicalWidth = physicalWidth;
            this.PhysicalHeight = physicalHeight;
        }

        /// <summary>
        /// Gets or sets the visible width of the display in pixels.
        /// This is the width of the visible area of the display.
        /// </summary>
        [XmlAttribute]
        public int VisibleWidth { get; set; }

        /// <summary>
        /// Gets or sets the visible height of the display in pixels.
        /// This is the height of the visible area of the display.
        /// </summary>
        [XmlAttribute]
        public int VisibleHeight { get; set; }

        /// <summary>
        /// Gets or sets the physical width of the display in pixels.
        /// This is the width of the actual display.
        /// </summary>
        [XmlAttribute]
        public int PhysicalWidth { get; set; }

        /// <summary>
        /// Gets or sets the physical height of the display in pixels.
        /// This is the height of the actual display.
        /// For wide screen this can differ from <see cref="VisibleHeight"/>.
        /// </summary>
        [XmlAttribute]
        public int PhysicalHeight { get; set; }
    }
}