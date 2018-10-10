namespace Luminator.PeripheralDimmer.Processor.Interfaces
{
    /// <summary>
    /// Interface for the dimmer processor input
    /// </summary>
    public interface IDimmerProcessorInput
    {
        /// <summary>
        /// Gets or sets minimum percent display brightness
        /// </summary>
        byte MinimumPercent { get; set; }

        /// <summary>
        /// Gets or sets maximumPercent display brightness
        /// </summary>
        byte MaximumPercent { get; set; }

        /// <summary>
        /// Gets or sets ambient light level
        /// </summary>
        ushort AmbientLightLevel { get; set; }

        /// <summary>
        /// Calculated ambient light lux
        /// </summary>
        float AmbientLightLux { get; set; }

        /// <summary>
        /// Gets or sets range scale
        /// </summary>
        byte RangeScale { get; set; }

        /// <summary>
        /// Gets or sets display brightness reading
        /// </summary>
        byte BrightnessLevel { get; set; }
    }
}
