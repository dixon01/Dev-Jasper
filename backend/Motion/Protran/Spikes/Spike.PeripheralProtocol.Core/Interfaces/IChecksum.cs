// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChecksum.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    /// <summary>The Checksum interface.</summary>
    public interface IChecksum
    {
        #region Public Properties

        /// <summary>Gets or sets the checksum.</summary>
        byte Checksum { get; set; }

        #endregion
    }
}