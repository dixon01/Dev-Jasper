// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralBaseMessage.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The PeripheralBaseMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using Luminator.PeripheralDimmer.Models;

    /// <summary>The PeripheralBaseMessage interface.</summary>
    public interface IPeripheralBaseMessage : IChecksum
    {
        #region Public Properties

        /// <summary>Gets or sets the header.</summary>
        PeripheralHeader Header { get; set; }

        #endregion
    }
}