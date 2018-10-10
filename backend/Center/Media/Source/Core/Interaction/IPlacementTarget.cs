// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlacementTarget.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPlacementTarget type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Media.Core.DataViewModels;

    /// <summary>
    /// The PlacementTarget interface.
    /// </summary>
    public interface IPlacementTarget
    {
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        DataValue<int> X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        DataValue<int> Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        DataValue<int> Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        DataValue<int> Height { get; set; }

        /// <summary>
        /// Gets or sets the is on canvas.
        /// </summary>
        DataValue<bool> UseMousePosition { get; set; }
    }
}