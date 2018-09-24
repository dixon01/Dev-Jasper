// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerMainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerMainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Main
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;

    /// <summary>
    /// The visualizer parent MDI form.
    /// </summary>
    public partial class VisualizerMainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerMainForm"/> class.
        /// </summary>
        public VisualizerMainForm()
        {
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
        }

        private void ExitToolsStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CascadeToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
