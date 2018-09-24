// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The main form for VDV 301 visualization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The main form for VDV 301 visualization.
    /// </summary>
    public partial class Vdv301MainForm : MdiChildBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301MainForm"/> class.
        /// </summary>
        public Vdv301MainForm()
        {
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            this.vdv301TabControl1.Configure(ServiceLocator.Current.GetInstance<IVdv301VisualizationService>());
        }
    }
}
