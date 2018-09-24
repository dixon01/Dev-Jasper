namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralRequestStatus : PeripheralAudioSwtichBaseMessage
    {
        public PeripheralRequestStatus() : base(PeripheralAudioSwitchMessageType.Poll, PeripheralSystemMessageType.AudioGeneration3)
        {
        }
        
    }
}