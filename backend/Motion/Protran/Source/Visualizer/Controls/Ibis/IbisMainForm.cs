// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisMainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Main window of IBIS Protran Visualizer
    /// </summary>
    public partial class IbisMainForm : MdiChildBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisMainForm"/> class.
        /// </summary>
        public IbisMainForm()
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

            this.ibisTabControl1.Configure(ServiceLocator.Current.GetInstance<IIbisVisualizationService>());
        }
    }
}
