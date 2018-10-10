namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerQueryRequest : DimmerBaseMessage, IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="DimmerQueryRequest" /> class.</summary>
        public DimmerQueryRequest()
            : base(DimmerMessageType.QueryRequest)
        {
        }
    }
}