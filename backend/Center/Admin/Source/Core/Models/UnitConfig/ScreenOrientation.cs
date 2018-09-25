// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenOrientation.cs" company="Luminator Technology Group">
//   Copyright © 2016 Luminator Technology Group. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenOrientation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Gorba.Common.Configuration.HardwareManager;

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The screen orientation.
    /// </summary>
    public class ScreenOrientation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gorba.Center.Admin.Core.Models.UnitConfig.ScreenOrientation"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the screen orientation mode.
        /// </param>
        /// <param name="mode">
        /// The visual width in pixels.
        /// </param>
        public ScreenOrientation(string name, OrientationMode mode)
        {
            Name = name;
            Mode = mode;
        }

        /// <summary>
        /// Gets the name of the screen orientation mode.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the screen orientation mode.
        /// </summary>
        public OrientationMode Mode { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(AdminStrings.UnitConfig_Hardware_ScreenOrientation_Format, Name, Mode);
        }
    }
}