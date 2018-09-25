// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutElementTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The LayoutElementTests.
    /// </summary>
    [CodedUITest]
    public class LayoutElementTests : UITestBase
    {
        /// <summary>
        /// Test to create a static text layout element
        /// </summary>
        [TestMethod, TestCategory("Ignore")]
        [Ignore]
        public void CreateStaticText()
        {
            var staticTextTool = this.UIMap.MediaShellWindow.EditorToolbar.StaticTextTool;
            
            Mouse.Click(staticTextTool);
            Assert.IsTrue(staticTextTool.Selected);

            var layoutEditor = this.UIMap.MediaShellWindow.LayoutEditor;
            layoutEditor.DrawRectangle(new Rect(50, 50, 200, 100));

            var layoutElement = new UIStaticTextLayoutElement(layoutEditor.LayoutPreviewRenderer);
            Mouse.DoubleClick(layoutElement);

            const string Text = "Some great Text Content";

            Keyboard.SendKeys(Text + "{Enter}");

            Assert.AreEqual(layoutElement.TextContainer.Text, Text, "The Text ist not as expected");
        }
    }
}