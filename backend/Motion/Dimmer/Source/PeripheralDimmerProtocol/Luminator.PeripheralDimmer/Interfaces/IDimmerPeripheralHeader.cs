// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IDimmerPeripheralHeader.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Interfaces
{
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The DimmerPeripheralHeader interface.</summary>
    public interface IDimmerPeripheralHeader : IPeripheralHeader
    {
        #region Public Properties

        /// <summary>Gets or sets the dimmer message type.</summary>
        DimmerMessageType DimmerMessageType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        string ToString();

        #endregion
    }
}