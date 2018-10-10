namespace Luminator.PeripheralDimmer.Processor.Interfaces
{
    /// <summary>
    /// Inteface for the dimmer processor
    /// </summary>
    public interface IDimmerProcessor
    {
        /// <summary>
        /// Gets or sets the tuning parameters
        /// </summary>
        IDimmerProcessorTuningParams TuningParams { get; set; }

        /// <summary>
        /// Gets the last dimmer processor input
        /// </summary>
        IDimmerProcessorInput LastDimmerInput { get; }

        /// <summary>
        /// Gets the last dimmer processor output
        /// </summary>
        IDimmerProcessorOutput LastDimmerOutput { get; }

        /// <summary>
        /// Dimmer output
        /// </summary>
        /// <param name="minimumPercent">MinimumPercent percentage 0...100</param>
        /// <param name="maximumPercent">MaximumPercent percentage 0...100</param>
        /// <param name="ambientLightLevel">0...65535</param>
        /// <param name="rangeScale">0..3</param>
        /// <param name="brightnessLevel">0...255</param>
        /// <returns></returns>
        IDimmerProcessorOutput CalculateDimmerOutput(
            byte minimumPercent,
            byte maximumPercent,
            ushort ambientLightLevel,
            byte rangeScale,
            byte brightnessLevel);
    }
}
