// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerAck.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer ack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer ack.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerAck : DimmerBaseMessage, IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="DimmerAck" /> class.</summary>
        public DimmerAck()
            : base(DimmerMessageType.Ack)
        {
        }
    }
}