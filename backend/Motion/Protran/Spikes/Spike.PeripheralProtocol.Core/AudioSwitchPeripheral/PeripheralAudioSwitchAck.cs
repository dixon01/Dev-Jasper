namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioSwitchAck : PeripheralAudioSwtichBaseMessage
    {
        public PeripheralAudioSwitchAck() :
            base(PeripheralAudioSwitchMessageType.Ack, PeripheralSystemMessageType.AudioGeneration3)
        {
        }
    }
}