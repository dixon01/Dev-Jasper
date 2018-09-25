// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateElementParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CreateElementParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Windows;

    /// <summary>
    /// Defines the set of parameters required to create a layout element
    /// </summary>
    public class CreateElementParameters
    {
        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public LayoutElementType Type { get; set; }

        /// <summary>
        /// Gets or sets the bounds
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// Gets or sets the modifiers
        /// </summary>
        public ModifiersState Modifiers { get; set; }

        /// <summary>
        /// Gets or sets the insert index of the target list.
        /// </summary>
        public int InsertIndex { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[CreateElementParameters Type: {0}, Bounds: {1}, modifiers: {2}]";
            return string.Format(Format, this.Type, this.Bounds, this.Modifiers);
        }
    }
}