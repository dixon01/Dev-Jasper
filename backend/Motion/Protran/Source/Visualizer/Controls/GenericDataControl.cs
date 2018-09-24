// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericDataControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericDataControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Controls;

    /// <summary>
    /// Control that shows the generic view data in a <see cref="GenericDataTabControl"/>
    /// and allows the user to clear the data.
    /// </summary>
    public partial class GenericDataControl : UserControl, IVisualizationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDataControl"/> class.
        /// </summary>
        public GenericDataControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="sideTab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab sideTab)
        {
        }

        private void ButtonClearClick(object sender, EventArgs e)
        {
            this.genericDataTabControl1.ClearXimple();
        }
    }
}
