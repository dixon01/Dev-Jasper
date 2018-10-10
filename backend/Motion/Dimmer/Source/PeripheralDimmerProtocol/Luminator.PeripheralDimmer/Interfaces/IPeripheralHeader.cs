// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralHeader.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The PeripheralHeader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    /// <summary>The PeripheralHeader interface.</summary>
    public interface IPeripheralHeader
    {
        #region Public Properties

        /// <summary>Gets or sets the address.</summary>
        ushort Address { get; set; }

        /// <summary>Gets or sets the length.</summary>
        ushort Length { get; set; }

        /// <summary>Gets or sets the message type.</summary>
        byte MessageType { get; set; }

        /// <summary>Gets or sets the system message type.</summary>
        byte SystemMessageType { get; set; }

        #endregion
    }
}