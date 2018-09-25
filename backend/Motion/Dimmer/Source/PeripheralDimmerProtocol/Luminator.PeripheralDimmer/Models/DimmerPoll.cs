namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer nak.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerPoll : DimmerBaseMessage, IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="Poll" /> class.</summary>
        public DimmerPoll()
            : base(DimmerMessageType.Poll)
        {
        }
    }
}