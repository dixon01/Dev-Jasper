namespace AudioSwitch.Test
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;

    /// <summary>The bogus peripheral base message.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal class BogusPeripheralBaseMessage : PeripheralAudioSwtichBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="BogusPeripheralBaseMessage"/> class.</summary>
    
        public BogusPeripheralBaseMessage()          
        {         
        }
        
    }
}