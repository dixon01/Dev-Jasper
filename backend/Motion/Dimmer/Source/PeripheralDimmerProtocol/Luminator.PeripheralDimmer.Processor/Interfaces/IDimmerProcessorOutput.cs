namespace Luminator.PeripheralDimmer.Processor.Interfaces
{
    /// <summary>
    /// Interface for the dimmer processor output
    /// </summary>
    public interface IDimmerProcessorOutput
    {
        /// <summary>
        /// Gets or sets new or existing range scale to set light sensor range to (0x00 to 0x03, Range1...Range4)
        /// </summary>
        byte RangeScale { get; set; }

        /// <summary>
        /// Gets or sets new brightness to set display to (0x00 to 0xFF), these are a series of settings to smoothen out 
        /// output IF NEEDED
        /// </summary>
        byte[] BrightnessSetting { get; set; }

        /// <summary>
        /// Gets or sets amount of time to pause between setting the brightness from first to last in milliseconds (ms)
        /// </summary>
        int IntervalDelay { get; set; }

        bool IsValid { get; }
    }
}
