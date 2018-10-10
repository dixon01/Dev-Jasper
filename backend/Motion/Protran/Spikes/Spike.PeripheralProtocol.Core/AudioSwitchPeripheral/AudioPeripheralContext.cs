namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System.IO;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    public sealed class AudioPeripheralContext : PeripheralContext<PeripheralAudioSwitchMessageType>
    {      
        public AudioPeripheralContext(string configFileName)
            : base(configFileName)
        {
        }

        public AudioPeripheralContext(PeripheralConfig config, Stream stream)
            : base(config, stream)
        {
        }

        public AudioPeripheralContext(PeripheralConfig config, IPeripheralSerialClient<PeripheralAudioSwitchMessageType> peripheralSerialClient)
            : base(config, peripheralSerialClient)
        {
        }
    }
}