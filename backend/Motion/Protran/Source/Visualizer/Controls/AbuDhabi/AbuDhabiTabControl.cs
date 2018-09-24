// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiTabControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiTabControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Tab control for Abu Dhabi visualization.
    /// </summary>
    public partial class AbuDhabiTabControl : UserControl
    {
        private readonly List<SideTab> sideTabs;
        private readonly List<Control> panels;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiTabControl"/> class.
        /// </summary>
        public AbuDhabiTabControl()
        {
            this.InitializeComponent();

            this.sideTabs = new List<SideTab>
                {
                    this.sideTab1, this.sideTab2, this.sideTab3, this.sideTab4
                };

            this.panels = new List<Control>
                {
                    this.dataItemsControl1,
                    this.abuDhabiTransformationsControl1,
                    this.abuDhabiGenericDataControl1,
                    this.abuDhabiLogsControl1
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

        private void OnSideTabClick(object sender, EventArgs e)
        {
            this.SelectTab((SideTab)sender);
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
    }
}
