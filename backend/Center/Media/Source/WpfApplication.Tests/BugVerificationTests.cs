// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BugVerificationTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The BugVerificationTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The LayoutElementTests.
    /// </summary>
    [CodedUITest]
    public class BugVerificationTests : UITestBase
    {
        /// <summary>
        /// Test for Bug 12723 
        /// </summary>
        [TestMethod, TestCategory("Ignore")]
        [Ignore]
        public void TestEditMenuNotProperlyClosingBug()
        {
            Mouse.Click(this.UIMap.MediaShellWindow.EditMenu, new System.Drawing.Point(25, 17));

            Assert.IsTrue(this.UIMap.MediaShellWindow.EditMenuDialog.Exists);

            Mouse.Click(this.UIMap.MediaShellWindow.LayoutEditor, new System.Drawing.Point(130, 365));

            this.UIMap.MediaShellWindow.EditMenuDialog.WaitForControlNotExist(2000);
        }
    }
}