// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerVersionRequest.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer version request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer version request.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerVersionRequest : DimmerBaseMessage, IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="DimmerVersionRequest" /> class.</summary>
        public DimmerVersionRequest()
            : base(DimmerMessageType.VersionRequest)
        {
        }
    }
}