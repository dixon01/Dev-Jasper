// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropTargetAdorners.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DropTargetAdorners.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;

    /// <summary>
    /// The drop target adorners
    /// </summary>
    public class DropTargetAdorners
    {
        /// <summary>
        /// Gets the highlight type
        /// </summary>
        public static Type Highlight
        {
            get { return typeof(DropTargetHighlightAdorner); }
        }

        /// <summary>
        /// Gets the insert type
        /// </summary>
        public static Type Insert
        {
            get { return typeof(DropTargetInsertionAdorner); }
        }
    }
}