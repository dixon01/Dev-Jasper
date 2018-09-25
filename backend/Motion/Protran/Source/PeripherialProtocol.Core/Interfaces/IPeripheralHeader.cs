// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IPeripheralHeader.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralHeader interface.</summary>
    public interface IPeripheralHeader
    {
        #region Public Properties

        /// <summary>Gets or sets the address.</summary>
        ushort Address { get; set; }

        /// <summary>Gets or sets the length.</summary>
        ushort Length { get; set; }

        /// <summary>Gets or sets the message type.</summary>
        PeripheralMessageType MessageType { get; set; }

        /// <summary>Gets or sets the system type.</summary>
        PeripheralSystemMessageType SystemType { get; set; }

        #endregion
    }
}