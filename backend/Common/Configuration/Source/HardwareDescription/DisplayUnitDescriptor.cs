// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayUnitDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System.Xml.Serialization;

    /// <summary>
    /// The descriptor for a physical display width optional info line.
    /// </summary>
    public class DisplayUnitDescriptor : DisplayDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitDescriptor"/> class.
        /// </summary>
        public DisplayUnitDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitDescriptor"/> class.
        /// </summary>
        /// <param name="index">
        /// The index of the display.
        /// </param>
        /// <param name="hasInfoLine">
        /// <c>true</c> if the display has an info line; <c>false</c> otherwise.
        /// </param>
        public DisplayUnitDescriptor(int index, bool hasInfoLine)
        {
            this.HasInfoline = hasInfoLine;
            this.Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitDescriptor"/> class.
        /// </summary>
        /// <param name="visibleWidth">
        /// The visible and physical width.
        /// </param>
        /// <param name="visibleHeight">
        /// The visible and physical height.
        /// </param>
        public DisplayUnitDescriptor(int visibleWidth, int visibleHeight)
            : this(visibleWidth, visibleHeight, visibleWidth, visibleHeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitDescriptor"/> class.
        /// </summary>
        /// <param name="visibleWidth">
        /// The visible width.
        /// </param>
        /// <param name="visibleHeight">
        /// The visible height.
        /// </param>
        /// <param name="physicalWidth">
        /// The physical width.
        /// </param>
        /// <param name="physicalHeight">
        /// The physical height.
        /// </param>
        public DisplayUnitDescriptor(int visibleWidth, int visibleHeight, int physicalWidth, int physicalHeight)
        {
            this.VisibleWidth = visibleWidth;
            this.VisibleHeight = visibleHeight;
            this.PhysicalWidth = physicalWidth;
            this.PhysicalHeight = physicalHeight;
        }

        /// <summary>
        /// Gets or sets the display index of the connected power unit.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the display unit has an info line.
        /// </summary>
        [XmlAttribute]
        public bool HasInfoline { get; set; }
    }
}
