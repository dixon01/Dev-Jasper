// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOTabControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOTabControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Data.IO;

    /// <summary>
    /// Tab control for IO Protocol.
    /// </summary>
    public partial class IOTabControl : UserControl
    {
        private readonly List<SideTab> sideTabs;

        private readonly List<Control> panels;

        /// <summary>
        /// Initializes a new instance of the <see cref="IOTabControl"/> class.
        /// </summary>
        public IOTabControl()
        {
            this.InitializeComponent();

            this.sideTabs = new List<SideTab>
                {
                    this.sideTab1, this.sideTab2, this.sideTab3, this.sideTab4
                };

            this.panels = new List<Control>
                {
                    this.buttonControl1,
                    this.ioTransformationControl1,
                    this.ioGenericDataControl1,
                    this.ioLogsControl1
                };

            for (int i = 0; i < this.sideTabs.Count; i++)
            {
                var control = this.panels[i] as IVisualizationControl;
                if (control != null)
                {
                    control.SetSideTab(this.sideTabs[i]);
                }
            }

            this.SelectTab(this.sideTabs[0]);
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Configure(IIOVisualizationService control)
        {
            this.buttonControl1.Configure(control);
            this.ioTransformationControl1.Configure(control);
            this.ioLogsControl1.Configure(control);
            this.ioGenericDataControl1.Configure(control);
        }

        private void SelectTab(SideTab tab)
        {
            this.SuspendLayout();
            try
            {
                for (int i = 0; i < this.sideTabs.Count; i++)
                {
                    var sideTab = this.sideTabs[i];
                    var panel = this.panels[i];

                    panel.Visible = sideTab.Selected = sideTab == tab;
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private void OnSideTabClick(object sender, EventArgs e)
        {
            this.SelectTab((SideTab)sender);
        }
    }
}
