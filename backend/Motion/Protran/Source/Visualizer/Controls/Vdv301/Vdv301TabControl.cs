// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301TabControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301TabControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    /// <summary>
    /// The VDV 301 tab control.
    /// </summary>
    public partial class Vdv301TabControl : UserControl
    {
        private readonly List<SideTab> sideTabs;
        private readonly List<Control> panels;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301TabControl"/> class.
        /// </summary>
        public Vdv301TabControl()
        {
            this.InitializeComponent();

            this.sideTabs = new List<SideTab>
                {
                    this.sideTab1, this.sideTab2, this.sideTab3, this.sideTab4
                };

            this.panels = new List<Control>
                {
                    this.inputControl,
                    this.transformationsControl,
                    this.genericDataControl,
                    this.logsControl
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
        /// Configure the control with the given service.
        /// </summary>
        /// <param name="service">
        /// The service to which you can for example attach event handlers
        /// </param>
        public void Configure(IVdv301VisualizationService service)
        {
            foreach (var panel in this.panels)
            {
                var control = panel as IVdv301VisualizationControl;
                if (control != null)
                {
                    control.Configure(service);
                }
            }
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
