namespace Luminator.PeripheralDimmer.Processor
{
    using System;

    using Luminator.PeripheralDimmer.Processor.Interfaces;

    /// <summary>
    /// Used by the dimmer processor to keep track of last input.
    /// </summary>
    [Serializable]
    public class DimmerProcessorInput : IDimmerProcessorInput
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets minimum percent display brightness
        /// </summary>
        public byte MinimumPercent { get; set; }

        /// <summary>
        /// Gets or sets maximumPercent display brightness
        /// </summary>
        public byte MaximumPercent { get; set; }

        /// <summary>
        /// Gets or sets ambient light level
        /// </summary>
        public ushort AmbientLightLevel { get; set; }

        /// <summary>
        /// Calculated ambient light lux
        /// </summary>
        public float AmbientLightLux { get; set; }

        /// <summary>
        /// Gets or sets light sensor range scale
        /// </summary>
        public byte RangeScale { get; set; }

        /// <summary>
        /// Gets or sets display brightness reading
        /// </summary>
        public byte BrightnessLevel { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DimmerProcessorInput()
        {

        }

        #endregion

        #region Public Methods and Operators
        #endregion

        #region Member Methods and Operators
        #endregion
    }
}
