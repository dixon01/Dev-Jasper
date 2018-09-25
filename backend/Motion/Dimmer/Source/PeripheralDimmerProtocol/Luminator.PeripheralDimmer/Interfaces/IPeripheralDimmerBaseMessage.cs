// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralDimmerBaseMessage.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The PeripheralDimmerBaseMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using System;
    using Luminator.PeripheralDimmer.Models;

    /// <summary>The PeripheralDimmerBaseMessage interface.</summary>
    public interface IPeripheralDimmerBaseMessage : IPeripheralHeader, IChecksum
    {
        /// <summary>Gets or sets the header.</summary>
        #region Public Properties

        /// <summary>Gets or sets the header.</summary>
        DimmerPeripheralHeader Header { get; set; }

        #endregion
    }
    
}