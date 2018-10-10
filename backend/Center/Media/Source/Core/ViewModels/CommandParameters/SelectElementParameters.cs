// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectElementParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The SelectElementParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Windows;

    /// <summary>
    /// Defines the set of parameters required to select a layout element
    /// </summary>
    public class SelectElementParameters
    {
        /// <summary>
        /// Gets or sets the bounds
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether clear selection.
        /// </summary>
        public bool ClearSelection { get; set; }

        /// <summary>
        /// Gets or sets the modifiers
        /// </summary>
        public ModifiersState Modifiers { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[SelectElementParameters Bounds: {0}, modifiers: {1}]";
            return string.Format(Format, this.Position, this.Modifiers);
        }
    }
}