// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveElementsCommandParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The MoveElementsCommandParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Windows;

    /// <summary>
    /// The MoveElementsCommandParameters.
    /// </summary>
    public class MoveElementsCommandParameters
    {
        /// <summary>
        /// Gets or sets the modifiers
        /// </summary>
        public ModifiersState Modifiers { get; set; }

        /// <summary>
        /// Gets or sets the Direction
        /// </summary>
        public MovementDirection? Direction { get; set; }

        /// <summary>
        /// Gets or sets the Delta
        /// </summary>
        public Vector Delta { get; set; }
    }
}