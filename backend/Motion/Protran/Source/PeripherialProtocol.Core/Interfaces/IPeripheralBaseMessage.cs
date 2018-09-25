// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralBaseMessage.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>The PeripheralBaseMessage interface.</summary>
    public interface IPeripheralBaseMessage : IChecksum
    {
        #region Public Properties

        /// <summary>Gets or sets the header.</summary>
        PeripheralHeader Header { get; set; }

        #endregion
    }
}