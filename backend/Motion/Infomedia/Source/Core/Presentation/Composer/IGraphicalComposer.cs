// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphicalComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IGraphicalComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Interface implemented by <see cref="GraphicalComposerBase{TElement}"/>.
    /// This interface is only used by <see cref="GraphicalComposerBase{TElement}"/> to access
    /// its children through a common interface (the generic argument is not known).
    /// </summary>
    internal interface IGraphicalComposer
    {
        /// <summary>
        /// Event for subclasses that is fired whenever the
        /// <see cref="GraphicalElementBase.VisibleProperty"/> changes
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Gets the X coordinate of this presenter. It takes
        /// into account the parent's X coordinate.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the Y coordinate of this presenter. It takes
        /// into account the parent's Y coordinate.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the width of this presenter. It takes
        /// into account the parent's width if available.
        /// This can be zero meaning this presenter has no horizontal bounds.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of this presenter. It takes
        /// into account the parent's height if available.
        /// This can be zero meaning this presenter has no vertical bounds.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Method to check if the visible property on this element is true.
        /// This method also checks the parent's enabled property.
        /// If no dynamic "Enabled" property is defined, this method returns true.
        /// Subclasses can override this method to provide additional
        /// evaluation (e.g. check if files are available or other conditions
        /// are met).
        /// </summary>
        /// <returns>
        /// true if there is no property defined or if the property evaluates to true is valid.
        /// </returns>
        bool IsVisible();
    }
}