namespace Luminator.PeripheralDimmer.Processor.Interfaces
{
    /// <summary>
    /// Interface for the dimmer tuning parameters
    /// </summary>
    public interface IDimmerProcessorTuningParams
    {
        /// <summary>
        /// The upper limit value in range 1 to switch to range 2
        /// </summary>
        ushort Range1Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 2 to switch to range 1
        /// </summary>
        ushort Range2Lower { get; set; }

        /// <summary>
        /// The upper limit value in range 2 to switch to range 3
        /// </summary>
        ushort Range2Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 3 to switch to range 2
        /// </summary>
        ushort Range3Lower { get; set; }

        /// <summary>
        /// The upper limit value in range 3 to switch to range 4
        /// </summary>
        ushort Range3Upper { get; set; }

        /// <summary>
        /// The lower limit value in range 4 to switch to range 3
        /// </summary>
        ushort Range4Lower { get; set; }

        /// <summary>
        /// The number of brightness steps to generate
        /// </summary>
        int Steps { get; set; }

        /// <summary>
        /// The number of milliseconds to delay between each set brightness
        /// </summary>
        int IntervalDelay { get; set; }
        
        /// <summary>
        /// Generally higher brightness value = higher brightness, this indicates the inverse behavior of the TFT
        /// </summary>
        bool InvertBrightness { get; set; }
        
        /// <summary>
        /// Puts the dimmer processor into debug mode
        /// </summary>
        bool DimmerProcessorDebugMode { get; set; }
    }
}
