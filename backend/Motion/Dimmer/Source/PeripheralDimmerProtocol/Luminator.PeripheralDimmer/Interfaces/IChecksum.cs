// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IChecksum.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The Checksum interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
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