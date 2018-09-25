namespace Luminator.PeripheralDimmer.Processor
{
    using System;

    using Luminator.PeripheralDimmer.Processor.Interfaces;

    /// <summary>
    /// Used by the dimmer processor to output what light sensor range scale and TFT brightness to set.
    /// </summary>
    [Serializable]
    public class DimmerProcessorOutput : IDimmerProcessorOutput
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets new or existing range scale to set light sensor range to (0x00 to 0x03, Range1...Range4)
        /// </summary>
        public byte RangeScale { get; set; }

        /// <summary>
        /// Gets or sets new brightness to set display to (0x00 to 0xFF), these are a series of settings to smoothen out 
        /// output IF NEEDED
        /// </summary>
        public byte[] BrightnessSetting { get; set; }

        /// <summary>
        /// Gets or sets amount of time to pause between setting the brightness from first to last in milliseconds (ms)
        /// </summary>
        public int IntervalDelay { get; set; }

        /// <summary>Gets a value indicating whether the values are valid.</summary>
        public bool IsValid
        {
            get
            {
                return this.IntervalDelay > 0 && (this.BrightnessSetting != null && this.BrightnessSetting.Length > 0);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default output
        /// </summary>
        public DimmerProcessorOutput()
        {

        }

        #endregion

        #region Public Methods and Operators
        #endregion

        #region Member Methods and Operators
        #endregion
    }
}

