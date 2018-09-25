// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UILayoutEditor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UILayoutEditor.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// The LayoutEditor.
    /// </summary>
    public partial class UILayoutEditor
    {
        /// <summary>
        /// The Draw rectangle helper function
        /// </summary>
        /// <param name="rect">the rectangle to be drawn</param>
        public void DrawRectangle(Rect rect)
        {
            Mouse.StartDragging(
                this.LayoutPreviewRenderer,
                new System.Drawing.Point((int)rect.TopLeft.X, (int)rect.TopLeft.Y));

            Mouse.StopDragging(
                this.LayoutPreviewRenderer,
                new System.Drawing.Point((int)rect.BottomRight.X, (int)rect.BottomRight.Y));
        }
    }
}