// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerBrightnessLevels.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System.Linq;

    using Luminator.PeripheralDimmer.Interfaces;

    /// <summary>The dimmer brightness levels.</summary>
    public class DimmerBrightnessLevels : IDimmerBrightnessLevels
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DimmerBrightnessLevels" /> class.</summary>
        public DimmerBrightnessLevels()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerBrightnessLevels"/> class.</summary>
        /// <param name="brightness">The brightness.</param>
        /// <param name="lightLevel">The light level.</param>
        /// <param name="lightSensorScale">The light sensor scale.</param>
        public DimmerBrightnessLevels(byte[] brightness, byte lightSensorScale, ushort lightLevel = 0)
        {
            this.Brightness = brightness;
            this.LightLevel = lightLevel;
            this.LightSensorScale = lightSensorScale;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the brightness.</summary>
        public byte [] Brightness { get; set; }

        /// <summary>Gets a value indicating whether is light sensor scale valid.</summary>
        public bool IsLightSensorScaleValid
        {
            get
            {
                return this.LightSensorScale >= 0 && this.LightSensorScale < 4;
            }
        }

        public bool IsBrightnessValid
        {
            get
            {
                return this.Brightness!=null && this.Brightness.Length > 0;
            }
        }

        /// <summary>Gets or sets the light level.</summary>
        public ushort LightLevel { get; set; }

        /// <summary>Gets or sets the light sensor scale.</summary>
        public byte LightSensorScale { get; set; }

        #endregion
    }
}