namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    public interface IPeripheralVersionsInfo
    {
        /// <summary>Gets or sets the hardware version text.</summary>
        string HardwareVersionText { get; set; }

        /// <summary>Gets or sets the serial number text.</summary>
        string SerialNumberText { get; set; }

        /// <summary>Gets or sets the software version text.</summary>
        string SoftwareVersionText { get; set; }
    }
}