namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioSwitchNak : PeripheralBaseMessage<PeripheralAudioSwitchMessageType>
    {
        public PeripheralAudioSwitchNak() :
              base(PeripheralAudioSwitchMessageType.Nak, PeripheralSystemMessageType.AudioGeneration3, Constants.DefaultPeripheralAudioSwitchAddress)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}