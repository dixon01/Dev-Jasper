
namespace Luminator.PeripheralDimmer.Interfaces
{
    public interface IDimmerBrightnessLevels
    {
        byte [] Brightness { get; set; }

        byte LightSensorScale { get; set; }

        ushort LightLevel { get; set; }
    }
}
