// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIMediaShellWindow.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UIMediaShellWindow.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

    /// <summary>
    /// The Media shell Window
    /// </summary>
    public partial class UIMediaShellWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIMediaShellWindow" /> class.
        /// </summary>
        public UIMediaShellWindow()
        {
            this.SearchProperties[WpfWindow.PropertyNames.Name] = "icenter Media";
            this.SearchProperties[WpfControl.PropertyNames.AutomationId] = "MediaShellWindow";
            this.SearchProperties.Add(
                new PropertyExpression(
                    UITestControl.PropertyNames.ClassName,
                    "HwndWrapper",
                    PropertyExpressionOperator.Contains));
        }
    }
}