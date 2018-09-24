namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System.IO;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    public class PeripheralContextAudioSwtich : PeripheralContext<PeripheralAudioSwitchMessageType>
    {
        public PeripheralContextAudioSwtich(string configFileName)
            : base(configFileName)
        {
        }

        public PeripheralContextAudioSwtich(PeripheralConfig config, Stream stream)
            : base(config, stream)
        {
        }

        public PeripheralContextAudioSwtich(PeripheralConfig config, IPeripheralSerialClient<PeripheralAudioSwitchMessageType> peripheralSerialClient)
            : base(config, peripheralSerialClient)
        {
        }
    }
}