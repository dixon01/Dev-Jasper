// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIMap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UIMap.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// The UI Map definition
    /// </summary>
    public class UIMap
    {
        private UIMediaShellWindow mediaShellWindow;

        /// <summary>
        /// Gets or or sets the Window
        /// </summary>
        public UIMediaShellWindow MediaShellWindow
        {
            get
            {
                if (this.mediaShellWindow == null)
                {
                    this.mediaShellWindow = new UIMediaShellWindow();
                }

                return this.mediaShellWindow;
            }
        }

        /// <summary>
        /// creates a static text layout element
        /// </summary>
        /// <param name="rect">the rectangle to define the text element</param>
        /// <returns>the created element</returns>
        public UIStaticTextLayoutElement CreateStaticTextElement(Rect rect)
        {
            var staticTextTool = this.MediaShellWindow.EditorToolbar.StaticTextTool;

            Mouse.Click(staticTextTool);

            var layoutEditor = this.MediaShellWindow.LayoutEditor;
            layoutEditor.DrawRectangle(rect);

            var layoutElement = new UIStaticTextLayoutElement(layoutEditor.LayoutPreviewRenderer);

            return layoutElement;
        }
    }
}
