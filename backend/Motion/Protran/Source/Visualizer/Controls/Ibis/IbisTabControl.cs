// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTabControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainTabControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// The main tab control containing all other visualization controls.
    /// </summary>
    public partial class IbisTabControl : UserControl, IIbisVisualizationControl
    {
        private readonly List<SideTab> sideTabs;
        private readonly List<Control> panels;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTabControl"/> class.
        /// </summary>
        public IbisTabControl()
        {
            this.InitializeComponent();

            this.sideTabs = new List<SideTab>
                {
                    this.sideTab1, this.sideTab2, this.sideTab3, this.sideTab4, this.sideTab5, this.sideTab6
                };

            this.panels = new List<Control>
                {
                    this.telegramControl1, 
                    this.parserControl1, 
                    this.transformationControl1, 
                    this.handlerControl1, 
                    this.genericDataControl1, 
                    this.logsControl1 
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
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="controller">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        public void Configure(IIbisVisualizationService controller)
        {
            foreach (var panel in this.panels)
            {
                var control = panel as IIbisVisualizationControl;
                if (control != null)
                {
                    control.Configure(controller);
                }
            }
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
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
