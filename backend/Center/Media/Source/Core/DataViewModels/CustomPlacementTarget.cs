// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomPlacementTarget.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomPlacementTarget type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// The custom placement target.
    /// </summary>
    internal class CustomPlacementTarget : IPlacementTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlacementTarget"/> class.
        /// </summary>
        public CustomPlacementTarget()
        {
            this.X = new DataValue<int>(0);
            this.Y = new DataValue<int>(0);
            this.Width = new DataValue<int>(0);
            this.Height = new DataValue<int>(0);
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public DataValue<int> X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public DataValue<int> Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public DataValue<int> Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public DataValue<int> Height { get; set; }

        /// <summary>
        /// Gets or sets the is on canvas.
        /// </summary>
        public DataValue<bool> UseMousePosition { get; set; }
    }
}