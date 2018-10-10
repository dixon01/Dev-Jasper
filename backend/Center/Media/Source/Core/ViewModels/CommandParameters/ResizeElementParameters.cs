// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeElementParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Defines the parameters needed to undo / redo a resize command.
    /// </summary>
    public class ResizeElementParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeElementParameters"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public ResizeElementParameters(GraphicalElementDataViewModelBase element)
        {
            this.Element = element;
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        public GraphicalElementDataViewModelBase Element { get; private set; }

        /// <summary>
        /// Gets or sets the old bounds.
        /// </summary>
        public Rect OldBounds { get; set; }

        /// <summary>
        /// Gets or sets the new bounds.
        /// </summary>
        public Rect NewBounds { get; set; }
    }
}
