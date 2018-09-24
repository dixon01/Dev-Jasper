// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeSnapData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EdgeSnapData.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The EdgeSnapData.
    /// </summary>
    public class EdgeSnapData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeSnapData"/> class.
        /// </summary>
        /// <param name="isHorizontal">determines if this is a Horizontal edge</param>
        public EdgeSnapData(bool isHorizontal)
        {
            this.IsHorizontal = isHorizontal;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a horizontal edge
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// Gets or sets the delta
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the distance
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Gets or sets the target element
        /// </summary>
        public GraphicalElementDataViewModelBase TargetElement { get; set; }
    }
}